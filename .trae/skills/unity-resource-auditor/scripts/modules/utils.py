import os
import yaml

class UnityLoader(yaml.SafeLoader):
    pass

def construct_unity_tag(loader, node):
    return loader.construct_scalar(node)

# Register a catch-all for !u! tags
try:
    yaml.add_constructor('!u!', construct_unity_tag, Loader=yaml.SafeLoader)
except:
    pass 

def get_file_size_mb(path):
    try:
        return os.path.getsize(path) / (1024 * 1024)
    except:
        return 0

def load_meta_data(meta_path):
    try:
        with open(meta_path, 'r') as f:
            content = f.read()
            # Clean up Unity YAML quirks
            content = content.replace('!u!', '')
            if content.startswith('%YAML'):
                try:
                    content = content.split('\n', 1)[1]
                except:
                    pass
            
            # Use SafeLoader
            return yaml.safe_load(content)
    except Exception as e:
        return None
