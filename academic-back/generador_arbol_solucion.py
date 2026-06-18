  # Ejecutar script : Ingresar ruta de la solucion y nombre de archivo de salida
  # python generador_arbol_solucion.py "D:\source\Academic" --output estructura_proyecto.txt

import os
import sys
import xml.etree.ElementTree as ET

def get_project_references(csproj_path):
    """
    Parsea un archivo .csproj para extraer las referencias a otros proyectos.
    """
    references = []
    try:
        # Asegurarse de que el XML se pueda parsear aunque contenga caracteres especiales
        tree = ET.parse(csproj_path)
        root = tree.getroot()

        # Usar la ruta correcta para encontrar los elementos ProjectReference
        for project_reference in root.findall('.//ProjectReference'):
            include_path = project_reference.get('Include')
            if include_path:
                # Extraer solo el nombre del proyecto del path
                project_name = os.path.splitext(os.path.basename(include_path))[0]
                references.append(project_name)
    except ET.ParseError as e:
        print(f"Error al parsear el archivo XML {csproj_path}: {e}")
    except Exception as e:
        print(f"Ocurrió un error inesperado con {csproj_path}: {e}")
    return references

def _generate_tree_recursive(current_path, project_refs, prefix, output_lines):
    """
    Función recursiva para generar la estructura del árbol.
    """
    # Excluir directorios no deseados
    items = [item for item in os.listdir(current_path) if item not in ['.vs', 'bin', 'obj']]
    items = sorted(items)
    
    # Separar directorios y archivos
    dirs = [item for item in items if os.path.isdir(os.path.join(current_path, item))]
    files = [item for item in items if os.path.isfile(os.path.join(current_path, item)) and item.endswith('.cs')]
    
    all_items = sorted(dirs) + sorted(files)
    
    for i, item in enumerate(all_items):
        path = os.path.join(current_path, item)
        is_last_item = (i == len(all_items) - 1)
        
        # Determinar el prefijo del árbol
        connector = '└──' if is_last_item else '├──'
        
        # Si es un directorio
        if os.path.isdir(path):
            is_project_folder = any(f.endswith('.csproj') for f in os.listdir(path))
            
            output_lines.append(f"{prefix}{connector} 📂 {item}")
            
            new_prefix = prefix + ('    ' if is_last_item else '│   ')

            if is_project_folder:
                # Si es un proyecto, imprimir sus referencias
                references = project_refs.get(item, [])
                if references:
                    for j, ref in enumerate(references):
                        is_last_ref = (j == len(references) - 1)
                        ref_connector = '└──' if is_last_ref else '├──'
                        output_lines.append(f"{new_prefix}{ref_connector} 📎 {ref}")
            
            _generate_tree_recursive(path, project_refs, new_prefix, output_lines)
            
        # Si es un archivo .cs
        elif item.endswith('.cs'):
            output_lines.append(f"{prefix}{connector} 📄 {item}")
            
def generate_tree(start_path, output_file=None):
    """
    Genera una representación de árbol del directorio y sus proyectos.
    """
    output_lines = []
    
    # Mapear todos los proyectos y sus referencias
    project_refs = {}
    for root, dirs, files in os.walk(start_path):
        if 'bin' in dirs: dirs.remove('bin')
        if 'obj' in dirs: dirs.remove('obj')
        if '.vs' in dirs: dirs.remove('.vs')
        
        for file_name in files:
            if file_name.endswith('.csproj'):
                csproj_path = os.path.join(root, file_name)
                project_name = os.path.splitext(file_name)[0]
                project_refs[project_name] = get_project_references(csproj_path)

    output_lines.append(f"Analizando: {os.path.basename(start_path)}")
    _generate_tree_recursive(start_path, project_refs, "", output_lines)
    
    # Imprimir o guardar la salida
    if output_file:
        try:
            with open(output_file, 'w', encoding='utf-8') as f:
                for line in output_lines:
                    f.write(line + '\n')
            print(f"\nLa estructura del proyecto se ha guardado en '{output_file}'.")
        except Exception as e:
            print(f"Error al escribir en el archivo '{output_file}': {e}")
    else:
        for line in output_lines:
            print(line)

def main():
    if len(sys.argv) < 2:
        print("Uso: python generador_arbol_solucion.py <ruta_del_directorio> [--output <nombre_archivo>]")
        sys.exit(1)

    start_path = sys.argv[1]
    output_file = None
    if len(sys.argv) > 2 and sys.argv[2] == "--output" and len(sys.argv) > 3:
        output_file = sys.argv[3]

    if not os.path.isdir(start_path):
        print(f"Error: La ruta '{start_path}' no es un directorio válido.")
        sys.exit(1)

    generate_tree(start_path, output_file)

if __name__ == "__main__":
    main()
