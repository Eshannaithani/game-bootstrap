// BuildRequestPoller.cs
// Request-file build helper, installed by game-bootstrap and ship. Editor only.
// Why a request file: BuildPipeline.BuildPlayer called from a tool or RPC thread fails with
// "user interactions not supported". A request written to disk is picked up on the next
// EditorApplication.update, runs on the main loop, and survives a domain reload in between.
// Request: <project>/Builds/build_request.json  { "target": "ios|android|webgl|mac", "output": "", "development": false, "scenes": "" }
// Report:  <project>/Builds/build_report.txt    REQUESTED / STARTED / RESULT / OUTPUT / errors / FINISHED lines
// Command line: Unity -batchmode -quit -projectPath . -executeMethod GameTools.BuildRequestPoller.BuildFromCommandLine -shipTarget android [-shipDev]
using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GameTools
{
    [InitializeOnLoad]
    public static class BuildRequestPoller
    {
        [Serializable]
        public class Request { public string target = "ios"; public string output = ""; public bool development = false; public string scenes = ""; }

        static string ProjectRoot => Directory.GetParent(Application.dataPath).FullName;
        public static string BuildsDir => Path.Combine(ProjectRoot, "Builds");
        public static string RequestPath => Path.Combine(BuildsDir, "build_request.json");
        public static string ReportPath => Path.Combine(BuildsDir, "build_report.txt");

        static BuildRequestPoller() { EditorApplication.update += Poll; }

        [MenuItem("Tools/Ship/Build iOS (Xcode project)")] static void MenuIos() { WriteRequest("ios", false); }
        [MenuItem("Tools/Ship/Build Android (APK)")] static void MenuAndroid() { WriteRequest("android", false); }
        [MenuItem("Tools/Ship/Build WebGL")] static void MenuWebGL() { WriteRequest("webgl", false); }

        public static void WriteRequest(string target, bool development, string output = "")
        {
            Directory.CreateDirectory(BuildsDir);
            var r = new Request { target = target, development = development, output = output ?? "" };
            File.WriteAllText(RequestPath, JsonUtility.ToJson(r, true));
            File.WriteAllText(ReportPath, "REQUESTED " + target + " " + DateTime.Now.ToString("HH:mm:ss") + "\n");
        }

        static void Poll()
        {
            if (!File.Exists(RequestPath)) return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlaying || BuildPipeline.isBuildingPlayer) return;
            Request request;
            try { request = JsonUtility.FromJson<Request>(File.ReadAllText(RequestPath)); }
            catch (Exception e) { File.AppendAllText(ReportPath, "BAD REQUEST " + e.Message + "\n"); File.Delete(RequestPath); return; }
            File.Delete(RequestPath);
            Build(request);
        }

        public static void Build(Request r)
        {
            var log = new StringBuilder();
            File.AppendAllText(ReportPath, "STARTED " + r.target + " " + DateTime.Now.ToString("HH:mm:ss") + "\n");
            try
            {
                BuildTarget target; BuildTargetGroup group; bool fileOutput = false;
                string output = r.output ?? "";
                string product = (PlayerSettings.productName ?? "Game").Replace(" ", "");
                switch ((r.target ?? "").ToLowerInvariant())
                {
                    case "ios": target = BuildTarget.iOS; group = BuildTargetGroup.iOS; if (output.Length == 0) output = Path.Combine(BuildsDir, "iOS"); break;
                    case "android": target = BuildTarget.Android; group = BuildTargetGroup.Android; fileOutput = true; if (output.Length == 0) output = Path.Combine(BuildsDir, "Android", product + ".apk"); break;
                    case "webgl": target = BuildTarget.WebGL; group = BuildTargetGroup.WebGL; if (output.Length == 0) output = Path.Combine(BuildsDir, "WebGL"); break;
                    case "mac": target = BuildTarget.StandaloneOSX; group = BuildTargetGroup.Standalone; fileOutput = true; if (output.Length == 0) output = Path.Combine(BuildsDir, "Mac", product + ".app"); break;
                    default: throw new ArgumentException("unknown target '" + r.target + "'");
                }
                Directory.CreateDirectory(fileOutput ? Path.GetDirectoryName(output) : output);
                string[] scenes = string.IsNullOrEmpty(r.scenes)
                    ? EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray()
                    : r.scenes.Split(';');
                var options = new BuildPlayerOptions
                {
                    scenes = scenes, locationPathName = output, target = target, targetGroup = group,
                    options = r.development ? BuildOptions.Development : BuildOptions.None,
                };
                BuildReport report = BuildPipeline.BuildPlayer(options);
                BuildSummary s = report.summary;
                log.AppendLine("RESULT " + s.result);
                log.AppendLine("OUTPUT " + s.outputPath);
                log.AppendLine("errors " + s.totalErrors + ", warnings " + s.totalWarnings + ", size " + s.totalSize + " bytes, time " + s.totalTime);
                foreach (BuildStep step in report.steps)
                    foreach (BuildStepMessage m in step.messages)
                        if (m.type == LogType.Error || m.type == LogType.Exception) log.AppendLine("[" + step.name + "] " + m.content);
            }
            catch (Exception e) { log.AppendLine("EXCEPTION " + e); }
            log.AppendLine("FINISHED " + DateTime.Now.ToString("HH:mm:ss"));
            File.AppendAllText(ReportPath, log.ToString());
        }

        public static void BuildFromCommandLine()
        {
            string[] args = Environment.GetCommandLineArgs();
            string target = "android"; bool dev = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-shipTarget" && i + 1 < args.Length) target = args[i + 1];
                if (args[i] == "-shipDev") dev = true;
            }
            Build(new Request { target = target, development = dev });
        }
    }
}
