
import re

def update_value(modification_lines, property_path, new_value):
    for i in range(len(modification_lines)):
        if f"propertyPath: {property_path}" in modification_lines[i]:
            # The value is on the next line
            line_parts = modification_lines[i+1].split(':')
            line_parts[1] = f" {new_value}"
            modification_lines[i+1] = ':'.join(line_parts)
            break

def get_value(modification_lines, property_path):
    for i in range(len(modification_lines)):
        if f"propertyPath: {property_path}" in modification_lines[i]:
            # The value is on the next line
            line_parts = modification_lines[i+1].split(':')
            return line_parts[1].strip()
    return None

def process_scene_file(file_path):
    with open(file_path, 'r') as f:
        content = f.read()

    documents = content.split('---')
    new_documents = []
    
    ground_guid = "85b7db77e9399184ca07f4b9a3beff87"
    deathzone_guid = "aedc001445a64ed4bb75a480d89c385a"
    player_transform_id = "31639889"

    ground_count = 0
    
    # New ground platform layouts
    ground_layouts = [
        {'pos': {'x': 0, 'y': -5, 'z': 0}, 'scale': {'x': 20, 'y': 1, 'z': 1}, 'rot': {'x': 0, 'y': 0, 'z': 0, 'w': 1}, 'euler': {'x': 0, 'y': 0, 'z': 0}},
        {'pos': {'x': 25, 'y': -5, 'z': 0}, 'scale': {'x': 20, 'y': 1, 'z': 1}, 'rot': {'x': 0, 'y': 0, 'z': 0, 'w': 1}, 'euler': {'x': 0, 'y': 0, 'z': 0}},
        {'pos': {'x': 50, 'y': -5, 'z': 0}, 'scale': {'x': 20, 'y': 1, 'z': 1}, 'rot': {'x': 0, 'y': 0, 'z': 0, 'w': 1}, 'euler': {'x': 0, 'y': 0, 'z': 0}},
        {'pos': {'x': 75, 'y': -5, 'z': 0}, 'scale': {'x': 20, 'y': 1, 'z': 1}, 'rot': {'x': 0, 'y': 0, 'z': 0, 'w': 1}, 'euler': {'x': 0, 'y': 0, 'z': 0}},
        {'pos': {'x': 100, 'y': -5, 'z': 0}, 'scale': {'x': 20, 'y': 1, 'z': 1}, 'rot': {'x': 0, 'y': 0, 'z': 0, 'w': 1}, 'euler': {'x': 0, 'y': 0, 'z': 0}},
    ]
    
    deathzone_layout = {'pos': {'x': 50, 'y': -20, 'z': 0}, 'scale': {'x': 120, 'y': 1, 'z': 1}}
    player_pos = {'x': 0, 'y': -4, 'z': 0}

    deathzone_modified = False

    for doc in documents:
        if not doc.strip():
            continue
            
        if ground_guid in doc:
            if ground_count < len(ground_layouts):
                layout = ground_layouts[ground_count] 
                
                # Split the document into lines to modify it
                lines = doc.strip().split('\n')
                
                # Find the modifications section
                try:
                    mod_start_index = next(i for i, line in enumerate(lines) if "m_Modifications:" in line)
                    
                    modification_lines = []
                    # Extract all modification blocks
                    mod_block_indices = [i for i, line in enumerate(lines) if 'target:' in line]
                    
                    for i in range(len(mod_block_indices)):
                        start = mod_block_indices[i]
                        end = mod_block_indices[i+1] if i+1 < len(mod_block_indices) else len(lines)
                        
                        prop_path_line = lines[start+1]
                        
                        if "propertyPath: m_LocalPosition.x" in prop_path_line:
                            lines[start+2] = f"      value: {layout['pos']['x']}"
                        elif "propertyPath: m_LocalPosition.y" in prop_path_line:
                            lines[start+2] = f"      value: {layout['pos']['y']}"
                        elif "propertyPath: m_LocalPosition.z" in prop_path_line:
                            lines[start+2] = f"      value: {layout['pos']['z']}"
                        elif "propertyPath: m_LocalScale.x" in prop_path_line:
                            lines[start+2] = f"      value: {layout['scale']['x']}"
                        elif "propertyPath: m_LocalScale.y" in prop_path_line:
                            lines[start+2] = f"      value: {layout['scale']['y']}"
                        elif "propertyPath: m_LocalScale.z" in prop_path_line:
                            lines[start+2] = f"      value: {layout['scale']['z']}"
                        elif "propertyPath: m_LocalRotation.x" in prop_path_line:
                            lines[start+2] = f"      value: {layout['rot']['x']}"
                        elif "propertyPath: m_LocalRotation.y" in prop_path_line:
                            lines[start+2] = f"      value: {layout['rot']['y']}"
                        elif "propertyPath: m_LocalRotation.z" in prop_path_line:
                            lines[start+2] = f"      value: {layout['rot']['z']}"
                        elif "propertyPath: m_LocalRotation.w" in prop_path_line:
                            lines[start+2] = f"      value: {layout['rot']['w']}"
                        elif "propertyPath: m_LocalEulerAnglesHint.x" in prop_path_line:
                             lines[start+2] = f"      value: {layout['euler']['x']}"
                        elif "propertyPath: m_LocalEulerAnglesHint.y" in prop_path_line:
                             lines[start+2] = f"      value: {layout['euler']['y']}"
                        elif "propertyPath: m_LocalEulerAnglesHint.z" in prop_path_line:
                             lines[start+2] = f"      value: {layout['euler']['z']}"
                            
                except StopIteration:
                    # No modifications found, we can't process this doc, so we keep it as is
                    new_documents.append(doc)
                    continue

                new_documents.append('\n'.join(lines))
                ground_count += 1
            else:
                # Discard extra ground prefabs
                pass
        
        elif deathzone_guid in doc and not deathzone_modified:
            lines = doc.strip().split('\n')
            try:
                mod_start_index = next(i for i, line in enumerate(lines) if "m_Modifications:" in line)
                mod_block_indices = [i for i, line in enumerate(lines) if 'target:' in line]
                
                for i in range(len(mod_block_indices)):
                    start = mod_block_indices[i]
                    end = mod_block_indices[i+1] if i+1 < len(mod_block_indices) else len(lines)
                    
                    prop_path_line = lines[start+1]
                    
                    if "propertyPath: m_LocalPosition.x" in prop_path_line:
                        lines[start+2] = f"      value: {deathzone_layout['pos']['x']}"
                    elif "propertyPath: m_LocalPosition.y" in prop_path_line:
                        lines[start+2] = f"      value: {deathzone_layout['pos']['y']}"
                    elif "propertyPath: m_LocalPosition.z" in prop_path_line:
                        lines[start+2] = f"      value: {deathzone_layout['pos']['z']}"
                    elif "propertyPath: m_LocalScale.x" in prop_path_line:
                        lines[start+2] = f"      value: {deathzone_layout['scale']['x']}"
                    elif "propertyPath: m_LocalScale.y" in prop_path_line:
                        lines[start+2] = f"      value: {deathzone_layout['scale']['y']}"

                new_documents.append('\n'.join(lines))
                deathzone_modified = True
            except StopIteration:
                new_documents.append(doc)

        elif f"&{player_transform_id}" in doc:
             lines = doc.strip().split('\n')
             for i, line in enumerate(lines):
                 if "m_LocalPosition" in line:
                     lines[i] = f"  m_LocalPosition: {{x: {player_pos['x']}, y: {player_pos['y']}, z: {player_pos['z']}}}"
                     break
             new_documents.append('\n'.join(lines))
        else:
            new_documents.append(doc)

    with open('Assets/Scenes/pdesingfinal.unity.new', 'w') as f:
        f.write('---\n'.join(new_documents))

process_scene_file('Assets/Scenes/pdesingfinal.unity')
