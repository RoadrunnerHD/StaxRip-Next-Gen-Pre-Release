import json
import runpy
import tempfile
from pathlib import Path
from unittest.mock import patch

script = Path(__file__).resolve().parents[1] / "Tools/DVOffset/scale_rpu.py"
with tempfile.TemporaryDirectory() as root:
    directory = Path(root) / "Film mit Umlauten ä"
    directory.mkdir()
    video = directory / "Film.hevc"
    video.write_bytes(b"test HEVC")
    metadata = directory / "HDRDVmetadata_L5.json"
    original = {"presets": [{"id": 0, "left": 12, "right": 14, "top": 20, "bottom": 22}], "edits": {"all": 0}}
    metadata.write_text(json.dumps(original))
    commands = []
    def run(command):
        commands.append(command)
        output = Path(command[command.index("-o") + 1])
        output.write_bytes(b"test output")
        return type("Result", (), {"returncode": 0})()
    with patch("builtins.input", side_effect=[str(video), "n"]), patch("subprocess.run", side_effect=run), patch("sys.argv", [str(script), str(Path(root) / "dovi_tool.exe")]):
        runpy.run_path(str(script), run_name="__main__")
    assert len(commands) == 3
    assert all(command[0] == str(Path(root) / "dovi_tool.exe") for command in commands)
    scaled = json.loads((directory / "dovi_work/rpu_scaled.json").read_text())
    assert scaled == {"mode": 2, "active_area": {"presets": [{"id": 0, "left": 6, "right": 7, "top": 10, "bottom": 11}], "edits": {"all": 0}}}
    assert json.loads(metadata.read_text()) == original
    assert video.read_bytes() == b"test HEVC"
    assert (directory / "Film_DV_Offset.hevc").exists()
    assert str(directory / "dovi_work/rpu_scaled.bin") in commands[-1]
print("PASS: JSON beside HEVC, absolute DoVi executable, original 0.5 scaling, output naming and no-deletion response.")
