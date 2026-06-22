import React, { useState, useEffect, useMemo } from 'react';
import { 
  View, 
  Text, 
  StyleSheet, 
  FlatList, 
  TouchableOpacity, 
  TextInput,
  Alert,
  ActivityIndicator
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import RNPickerSelect from 'react-native-picker-select';

const API_URL = 'https://cloudacademic.onrender.com/api';

export const MatriculaCarreraListScreen = ({ navigation }) => {
  const [matriculas, setMatriculas] = useState([]);
  const [promociones, setPromociones] = useState([]);
  const [promocionSeleccionada, setPromocionSeleccionada] = useState(0);
  const [busqueda, setBusqueda] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    cargarPromociones();
  }, []);

  const cargarPromociones = async () => {
    try {
      // CORRECCIÓN: maestros en minúscula
      const response = await fetch(`${API_URL}/maestros/promociones`, {
        headers: { 
          'Accept': 'application/json',
          'X-Tunnel-Skip-AntiPhishing-Page': 'true',
          'ngrok-skip-browser-warning': 'true'
        }
      });
      if (!response.ok) throw new Error('Error al cargar promociones');
      
      const data = await response.json();
      
      const opcionTodos = { id: 0, nombre: '-- Todas las Promociones --' };
      const listaConTodos = [opcionTodos, ...data];
      
      setPromociones(listaConTodos);
      cargarMatriculas(0);
    } catch (error) {
      console.error("Error en cargarPromociones:", error);
      setPromociones([{ id: 0, nombre: '-- Todas las Promociones --' }]);
      cargarMatriculas(0);
    }
  };

  const cargarMatriculas = async (idPromocion) => {
    setIsLoading(true);
    try {
      // CORRECCIÓN: matriculas en minúscula
      const response = await fetch(`${API_URL}/matriculas/promocion/${idPromocion}`, {
        headers: { 
          'Accept': 'application/json',
          'X-Tunnel-Skip-AntiPhishing-Page': 'true',
          'ngrok-skip-browser-warning': 'true'
        }
      });
      if (!response.ok) throw new Error('Error al cargar matrículas');
      
      const data = await response.json();
      setMatriculas(data);
    } catch (error) {
      console.error("Error en cargarMatriculas:", error);
      setMatriculas([]);
    } finally {
      setIsLoading(false);
    }
  };

  const onPromocionChange = (idPromocion) => {
    setPromocionSeleccionada(idPromocion);
    if (idPromocion !== null && idPromocion !== undefined) {
      cargarMatriculas(idPromocion);
    } else {
      setMatriculas([]);
    }
  };

  const confirmarEliminacion = (idMatricula) => {
    Alert.alert(
      "Confirmar Eliminación",
      "¿Está seguro de que desea remover esta matrícula?",
      [
        { text: "Cancelar", style: "cancel" },
        { 
          text: "Sí, remover", 
          style: "destructive",
          onPress: () => {
            Alert.alert("Confirmado", "Matrícula removida exitosamente.");
          }
        }
      ]
    );
  };

  const matriculasFiltradas = useMemo(() => {
    if (!busqueda) return matriculas;
    const lowerBusqueda = busqueda.toLowerCase();
    return matriculas.filter(mat => {
      const idAlumno = (mat?.idalumno || mat?.IdAlumno || '').toString().toLowerCase();
      const nombre = (mat?.nombrecompleto || mat?.NombreCompleto || '').toLowerCase();
      const grupo = (mat?.grupocodigo || mat?.GrupoCodigo || '').toLowerCase();
      return idAlumno.includes(lowerBusqueda) || nombre.includes(lowerBusqueda) || grupo.includes(lowerBusqueda);
    });
  }, [matriculas, busqueda]);

  const MatriculaItem = ({ item }) => {
    const idAlumno = (item?.idalumno || item?.Idalumno || item?.IdAlumno)?.toString()?.padStart(6, '0') || '000000';
    const nombre = item?.nombrecompleto || item?.Nombrecompleto || item?.NombreCompleto || 'Sin nombre registrado';
    const grupo = item?.grupocodigo || item?.Grupocodigo || item?.GrupoCodigo || 'N/A';
    const seccion = item?.seccion || item?.Seccion || '[Sin Sección]';
    const esMatricula = item?.esmatricula || item?.Esmatricula || item?.EsMatricula;
    const idMatricula = item?.idmatricula || item?.Idmatricula || item?.IdMatricula;

    return (
      <View style={styles.tableRow}>
        <View style={styles.colAlumno}>
          <Text style={styles.idText}>{idAlumno}</Text>
          <Text style={styles.nameText} numberOfLines={2}>{nombre}</Text>
        </View>
        <View style={styles.colGrupo}>
          <Text style={styles.grupoText}>Grupo: {grupo}</Text>
          <Text style={styles.subText}>{seccion}</Text>
        </View>
        <View style={styles.colEstado}>
          <View style={[styles.estadoBadge, esMatricula ? styles.badgeSuccess : styles.badgeWarning]}>
            <View style={[styles.estadoDot, esMatricula ? styles.dotSuccess : styles.dotWarning]} />
            <Text style={[styles.estadoText, esMatricula ? styles.textSuccess : styles.textWarning]}>
              {esMatricula ? 'Matriculado' : 'Pendiente'}
            </Text>
          </View>
        </View>
        <View style={styles.colAcciones}>
          <TouchableOpacity style={styles.actionBtn} onPress={() => navigation.navigate('MatriculaCarreraForm', { id: idMatricula })}>
            <Ionicons name="pencil" size={20} color="#6366F1" />
          </TouchableOpacity>
          <TouchableOpacity style={styles.actionBtn} onPress={() => confirmarEliminacion(idMatricula)}>
            <Ionicons name="trash" size={20} color="#EF4444" />
          </TouchableOpacity>
        </View>
      </View>
    );
  };

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <View style={styles.headerText}>
          <Text style={styles.title}>Gestión de Matrículas de Carreras</Text>
          <Text style={styles.subtitle}>Gestione las inscripciones, grupos y promociones de los alumnos.</Text>
        </View>
        <TouchableOpacity style={styles.addButton} onPress={() => navigation.navigate('MatriculaCarreraForm')}>
          <Ionicons name="add" size={20} color="#FFFFFF" />
          <Text style={styles.addButtonText}>Nueva Matrícula</Text>
        </TouchableOpacity>
      </View>

      <View style={styles.filters}>
        <View style={styles.pickerWrapper}>
          <Text style={styles.pickerLabel}>Promoción:</Text>
          <View style={styles.pickerContainer}>
            <RNPickerSelect
              placeholder={{}} 
              value={promocionSeleccionada}
              onValueChange={onPromocionChange}
              items={promociones.map(p => ({ label: p.nombre || p.Nombre, value: p.id || p.Id }))}
              style={pickerStyles}
            />
          </View>
        </View>
        
        <View style={styles.searchContainer}>
          <Ionicons name="search" size={18} color="#94A3B8" style={styles.searchIcon} />
          <TextInput
            style={styles.searchInput}
            placeholder="Buscar por Nombre, ID, o Grupo..."
            placeholderTextColor="#94A3B8"
            value={busqueda}
            onChangeText={setBusqueda}
          />
        </View>
      </View>

      <View style={styles.tableHeader}>
        <Text style={[styles.th, styles.colAlumno]}>ALUMNO</Text>
        <Text style={[styles.th, styles.colGrupo]}>GRUPO / SECCIÓN</Text>
        <Text style={[styles.th, styles.colEstado, { textAlign: 'center' }]}>ESTADO</Text>
        <Text style={[styles.th, styles.colAcciones]}></Text>
      </View>

      {isLoading ? (
        <View style={styles.centerContent}>
          <ActivityIndicator size="large" color="#4F46E5" />
        </View>
      ) : matriculasFiltradas.length === 0 ? (
        <View style={styles.centerContent}>
          <Ionicons name="people-outline" size={60} color="#CBD5E1" />
          <Text style={styles.emptyTitle}>No hay matrículas registradas</Text>
          <Text style={styles.emptySubtitle}>No se encontraron alumnos para esta promoción o los filtros de búsqueda.</Text>
        </View>
      ) : (
        <FlatList
          data={matriculasFiltradas}
          renderItem={({ item }) => <MatriculaItem item={item} />}
          keyExtractor={(_, index) => index.toString()}
          contentContainerStyle={styles.listContent}
        />
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#F8FAFC' },
  header: { padding: 20, backgroundColor: '#FFFFFF', borderBottomWidth: 1, borderColor: '#E2E8F0' },
  headerText: { marginBottom: 15 },
  title: { fontSize: 20, fontWeight: 'bold', color: '#0F172A' },
  subtitle: { fontSize: 13, color: '#64748B', marginTop: 4 },
  addButton: { backgroundColor: '#4F46E5', flexDirection: 'row', alignItems: 'center', justifyContent: 'center', paddingVertical: 10, borderRadius: 8 },
  addButtonText: { color: '#FFFFFF', fontWeight: 'bold', marginLeft: 8 },
  filters: { padding: 15, backgroundColor: '#FFFFFF', borderBottomWidth: 1, borderColor: '#E2E8F0' },
  pickerWrapper: { marginBottom: 12 },
  pickerLabel: { fontSize: 13, fontWeight: '600', color: '#334155', marginBottom: 6 },
  pickerContainer: { borderWidth: 1, borderColor: '#CBD5E1', borderRadius: 8, backgroundColor: '#F8FAFC' },
  searchContainer: { flexDirection: 'row', alignItems: 'center', borderWidth: 1, borderColor: '#CBD5E1', borderRadius: 8, paddingHorizontal: 12, backgroundColor: '#FFFFFF' },
  searchIcon: { marginRight: 8 },
  searchInput: { flex: 1, height: 44, color: '#0F172A' },
  tableHeader: { flexDirection: 'row', paddingHorizontal: 15, paddingVertical: 12, backgroundColor: '#F1F5F9', borderBottomWidth: 1, borderColor: '#E2E8F0' },
  th: { fontSize: 11, fontWeight: 'bold', color: '#64748B', textTransform: 'uppercase' },
  tableRow: { flexDirection: 'row', alignItems: 'center', paddingHorizontal: 15, paddingVertical: 14, backgroundColor: '#FFFFFF', borderBottomWidth: 1, borderColor: '#F1F5F9' },
  colAlumno: { flex: 3 },
  colGrupo: { flex: 2 },
  colEstado: { flex: 2, alignItems: 'center' },
  colAcciones: { flex: 1.5, flexDirection: 'row', justifyContent: 'flex-end' },
  idText: { fontSize: 11, color: '#4F46E5', backgroundColor: '#EEF2FF', alignSelf: 'flex-start', paddingHorizontal: 6, paddingVertical: 2, borderRadius: 4, fontWeight: 'bold', marginBottom: 4 },
  nameText: { fontSize: 13, fontWeight: '600', color: '#1E293B' },
  grupoText: { fontSize: 13, fontWeight: '600', color: '#334155' },
  subText: { fontSize: 11, color: '#64748B' },
  estadoBadge: { flexDirection: 'row', alignItems: 'center', paddingHorizontal: 8, paddingVertical: 4, borderRadius: 20, borderWidth: 1 },
  badgeSuccess: { backgroundColor: '#ECFDF5', borderColor: '#A7F3D0' },
  badgeWarning: { backgroundColor: '#FFFBEB', borderColor: '#FDE68A' },
  dotSuccess: { width: 6, height: 6, borderRadius: 3, backgroundColor: '#10B981', marginRight: 4 },
  dotWarning: { width: 6, height: 6, borderRadius: 3, backgroundColor: '#F59E0B', marginRight: 4 },
  textSuccess: { fontSize: 11, fontWeight: '600', color: '#047857' },
  textWarning: { fontSize: 11, fontWeight: '600', color: '#B45309' },
  actionBtn: { padding: 6, marginLeft: 8 },
  centerContent: { flex: 1, justifyContent: 'center', alignItems: 'center', padding: 20 },
  emptyTitle: { fontSize: 16, fontWeight: 'bold', color: '#0F172A', marginTop: 15 },
  emptySubtitle: { fontSize: 13, color: '#64748B', textAlign: 'center', marginTop: 5 },
  listContent: { paddingBottom: 20 }
});

const pickerStyles = StyleSheet.create({
  inputIOS: { 
    fontSize: 14,
    paddingVertical: 14,
    paddingHorizontal: 12, 
    color: '#0F172A',
    paddingRight: 30, 
  },
  inputAndroid: { 
    fontSize: 14,
    paddingHorizontal: 12, 
    paddingVertical: 14, 
    color: '#0F172A',
    paddingRight: 30,
    minHeight: 50, 
  },
});