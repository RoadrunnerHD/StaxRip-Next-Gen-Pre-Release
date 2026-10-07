import json
import sys
import subprocess
import shutil
from pathlib import Path
import traceback

SCALE = 0.5
DOVI_TOOL = sys.argv[1] if len(sys.argv) > 1 else "dovi_tool.exe"

def run(cmd):
    p = subprocess.run(cmd)
    if p.returncode != 0:
        raise RuntimeError(f"Error: {' '.join(cmd)}")

def scale_value(v):
    return int(round(v * SCALE))

def normalize_json(data):
    if "active_area" in data:
        presets = data["active_area"].get("presets", [])
        edits = data["active_area"].get("edits", {})
    else:
        presets = data.get("presets", [])
        edits = data.get("edits", {})

    for p in presets:
        for k in ["left", "right", "top", "bottom"]:
            if k in p:
                p[k] = scale_value(p[k])

    return {
        "mode": 2,
        "active_area": {
            "presets": presets,
            "edits": edits
        }
    }

def main():
    hevc_input = input("Drag the HEVC file here and press Enter: ").strip().strip('"')

    hevc_input = Path(hevc_input).resolve()
    base = hevc_input.parent
    workdir = base / "dovi_work"

    if workdir.exists():
        shutil.rmtree(workdir, ignore_errors=True)

    workdir.mkdir(exist_ok=True)

    json_file = base / "HDRDVmetadata_L5.json"
    rpu = workdir / "rpu.bin"
    json_scaled = workdir / "rpu_scaled.json"
    rpu_scaled = workdir / "rpu_scaled.bin"

    output_hevc = hevc_input.parent / f"{hevc_input.stem}_DV_Offset.hevc"

    print("\n[1] extract RPU")

    run([
        DOVI_TOOL,
        "extract-rpu",
        "-i",
        str(hevc_input),
        "-o",
        str(rpu)
    ])

    if not json_file.exists():
        raise FileNotFoundError("HDRDVmetadata_L5.json is missing")

    print("\n[2] scale metadata")

    data = json.loads(json_file.read_text(encoding="utf-8"))
    fixed = normalize_json(data)

    json_scaled.write_text(
        json.dumps(fixed, indent=2),
        encoding="utf-8"
    )

    print("\n[3] rebuild RPU")

    run([
        DOVI_TOOL,
        "editor",
        "-i",
        str(rpu),
        "-j",
        str(json_scaled),
        "-o",
        str(rpu_scaled)
    ])

    print("\n[4] inject corrected RPU")

    run([
        DOVI_TOOL,
        "inject-rpu",
        "-i",
        str(hevc_input),
        "--rpu-in",
        str(rpu_scaled),
        "-o",
        str(output_hevc)
    ])

    print("\nDONE:")
    print(output_hevc)

    delete_files = input(
        "\nDelete the original file and HDRDVmetadata_L5.json? (y/n): "
    ).strip().lower()

    if delete_files in ("j", "ja", "y", "yes"):
        if hevc_input.exists():
            hevc_input.unlink()
            print(f"Deleted: {hevc_input}")

        if json_file.exists():
            json_file.unlink()
            print(f"Deleted: {json_file}")

        shutil.rmtree(workdir, ignore_errors=True)

if __name__ == "__main__":
    try:
        main()
    except Exception:
        print("\nERROR:")
        traceback.print_exc()
        input("\nPress ENTER to close...")