import os
import sys
import datetime

# Add the current directory to sys.path to ensure modules can be imported
current_dir = os.path.dirname(os.path.abspath(__file__))
sys.path.append(current_dir)

from modules.utils import get_file_size_mb, load_meta_data
from modules.analyzers import analyze_texture_importer, analyze_model_importer, analyze_audio_importer
from modules.reporter import save_reports

def main():
    assets_path = "Assets"
    report = {
        'summary': {'count': {}, 'size_mb': {}},
    }
    all_assets = []
    
    print(f"Analyzing assets in {assets_path}...")

    for root, dirs, files in os.walk(assets_path):
        for file in files:
            if file.endswith(".meta"):
                continue
                
            file_path = os.path.join(root, file)
            meta_path = file_path + ".meta"
            
            ext = os.path.splitext(file)[1].lower()
            asset_type = "Other"
            
            if ext in ['.png', '.jpg', '.jpeg', '.tga', '.psd', '.tif']: asset_type = "Texture"
            elif ext in ['.fbx', '.obj', '.blend', '.dae']: asset_type = "Model"
            elif ext in ['.mp3', '.wav', '.ogg', '.aiff']: asset_type = "Audio"
            elif ext in ['.cs']: asset_type = "Script"
            elif ext in ['.mat']: asset_type = "Material"
            elif ext in ['.prefab']: asset_type = "Prefab"
            elif ext in ['.unity']: asset_type = "Scene"
            
            if asset_type == "Other": continue

            # Gather Asset Info
            size = get_file_size_mb(file_path)
            asset_info = {
                'path': file_path,
                'type': asset_type,
                'size_mb': size,
                'issues': []
            }

            # Update Summary
            report['summary']['count'][asset_type] = report['summary']['count'].get(asset_type, 0) + 1
            report['summary']['size_mb'][asset_type] = report['summary']['size_mb'].get(asset_type, 0.0) + size

            # Analyze Meta if exists
            if os.path.exists(meta_path) and asset_type in ["Texture", "Model", "Audio"]:
                meta_data = load_meta_data(meta_path)
                if meta_data:
                    if asset_type == "Texture": analyze_texture_importer(meta_data, file_path, asset_info['issues'])
                    elif asset_type == "Model": analyze_model_importer(meta_data, file_path, asset_info['issues'])
                    elif asset_type == "Audio": analyze_audio_importer(meta_data, file_path, asset_info['issues'])
            
            all_assets.append(asset_info)

    # Generate and Save Reports
    save_reports(report, all_assets)

if __name__ == "__main__":
    main()
