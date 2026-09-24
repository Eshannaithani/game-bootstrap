#!/usr/bin/env python3
"""Copy the game-bootstrap scaffold into a Unity project with placeholders substituted.

Usage:
  scaffold.py <project> --name "My Game" --company "My Studio" [--namespace MyGame] [--bundle-id com.mystudio.mygame] [--dry-run]

Never overwrites: when a target exists and differs, the new content is written to <file>.bootstrap-new
so the user can merge. Prints a JSON summary of created, identical, differing and skipped files.
"""
import argparse, json, os, re, sys

HERE = os.path.dirname(os.path.abspath(__file__))
SCAFFOLD = os.path.join(os.path.dirname(HERE), "assets", "scaffold")


def pascal(s):
    return re.sub(r"[^A-Za-z0-9]", "", "".join(w[:1].upper() + w[1:] for w in re.split(r"[\s_\-]+", s))) or "Game"


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("project")
    ap.add_argument("--name", required=True)
    ap.add_argument("--company", required=True)
    ap.add_argument("--namespace", default="")
    ap.add_argument("--bundle-id", default="")
    ap.add_argument("--dry-run", action="store_true")
    a = ap.parse_args()

    ns = a.namespace or pascal(a.name)
    bundle = a.bundle_id or "com.%s.%s" % (re.sub(r"[^a-z0-9]", "", a.company.lower()) or "studio", re.sub(r"[^a-z0-9]", "", a.name.lower()) or "game")
    subs = {"__GAME__": a.name, "__COMPANY__": a.company, "__NAMESPACE__": ns, "__BUNDLE__": bundle}
    summary = {"project": a.project, "namespace": ns, "bundleId": bundle, "created": [], "identical": [], "differs": [], "dry_run": a.dry_run}

    for root, _, files in os.walk(SCAFFOLD):
        for f in files:
            src = os.path.join(root, f)
            rel = os.path.relpath(src, SCAFFOLD)
            dst = os.path.join(a.project, rel)
            with open(src, "r", encoding="utf-8") as fh:
                content = fh.read()
            for k, v in subs.items():
                content = content.replace(k, v)
            if os.path.exists(dst):
                with open(dst, "r", encoding="utf-8", errors="replace") as fh:
                    existing = fh.read()
                if existing == content:
                    summary["identical"].append(rel)
                else:
                    summary["differs"].append(rel)
                    if not a.dry_run:
                        with open(dst + ".bootstrap-new", "w", encoding="utf-8") as fh:
                            fh.write(content)
                continue
            summary["created"].append(rel)
            if not a.dry_run:
                os.makedirs(os.path.dirname(dst), exist_ok=True)
                with open(dst, "w", encoding="utf-8") as fh:
                    fh.write(content)

    print(json.dumps(summary, indent=2))
    return 0


if __name__ == "__main__":
    sys.exit(main())
