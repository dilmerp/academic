import React, { useState, useEffect } from 'react';
import { 
  View, 
  Text, 
  StyleSheet, 
  ScrollView, 
  TouchableOpacity, 
  TextInput,
  Alert,
  ActivityIndicator
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import RNPickerSelect from 'react-native-picker-select';

const API_URL = 'https://cloudacademic.onrender.com/api';

export const MatriculaCarreraFormScreen = ({ navigation, route }) => {
  const isEditing = route.params?.id !== undefined;

  // Estados del Formulario
  const [idPromocion, setIdPromocion] = useState(null);
  const [idGrupo, setIdGrupo] = useState(null);
  const [idCurso, setIdCurso] = useState(null);
  const [idPlanPago, setIdPlanPago] = useState(null);
  const [idBeca, setIdBeca] = useState(null);
  const [usuarioModificacion] = useState(1);
  
  // Estados de datos (Catálogos)
  const [promociones, setPromociones] = useState([]);
  const [grupos, setGrupos] = useState([]);
  const [cursos, setCursos] = useState([]);
  const [planesPago, setPlanesPago] = useState([]);
  const [becas, setBecas] = useState([]);

  // Estados del Autocomplete de Alumnos
  const [alumnoBusqueda, setAlumnoBusqueda] = useState('');
  const [alumnosFiltrados, setAlumnosFiltrados] = useState([]);
  const [alumnoSeleccionado, setAlumnoSeleccionado] = useState(null);
  const [isSearching, setIsSearching] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const secureHeaders = {
    'Accept': 'application/json',
    'X-Tunnel-Skip-AntiPhishing-Page': 'true',
    'ngrok-skip-browser-warning': 'true'
  };

  const fetchData = async (url, setter) => {
    try {
      const res = await fetch(url, { headers: secureHeaders });
      if (res.ok) {
        const data = await res.json();
        setter(data);
      } else {
        setter([]);
      }
    } catch (err) {
      console.error('Error fetching:', url, err);
      setter([]);
    }
  };

  useEffect(() => {
    cargarCatalogosMaestros();
  }, []);

  useEffect(() => {
    setIdGrupo(null);
    setIdPlanPago(null);
    setIdCurso(null);
    setGrupos([]);
    setCursos([]);
    
    if (idPromocion) {
      // CORRECCIÓN: maestros en minúscula
      fetchData(`${API_URL}/maestros/promociones/${idPromocion}/grupos`, setGrupos);
      fetchData(`${API_URL}/maestros/promociones/${idPromocion}/cursos`, setCursos);
      verificarBecas(idPromocion, alumnoSeleccionado);
    }
  }, [idPromocion]);

  useEffect(() => {
    setIdPlanPago(null);
    setPlanesPago([]);
    if (idPromocion && idGrupo) {
      // CORRECCIÓN: maestros en minúscula
      fetchData(`${API_URL}/maestros/planes-pago?idPromocion=${idPromocion}&idGrupo=${idGrupo}&idSeccion=-1`, setPlanesPago);
    }
  }, [idGrupo]);

  const cargarCatalogosMaestros = () => {
    // CORRECCIÓN: maestros en minúscula
    fetchData(`${API_URL}/maestros/promociones`, setPromociones);
  };

  const verificarBecas = async (promo, alumno) => {
    const idActor = alumno?.idActor || alumno?.IdActor;
    if (promo && idActor) {
      // CORRECCIÓN: maestros en minúscula
      fetchData(`${API_URL}/maestros/becas?idActor=${idActor}&idPromocion=${promo}`, setBecas);
    } else {
      setBecas([]);
    }
  };

  const buscarAlumno = async (query) => {
    setAlumnoBusqueda(query);
    if (query.length < 3) {
      setAlumnosFiltrados([]);
      return;
    }
    
    setIsSearching(true);
    try {
      // CORRECCIÓN: alumnos en minúscula
      const res = await fetch(`${API_URL}/alumnos/buscar?termino=${query}`, { headers: secureHeaders });
      if (res.ok) {
        const data = await res.json();
        setAlumnosFiltrados(data);
      }
    } catch (err) {
      console.error(err);
    } finally {
      setIsSearching(false);
    }
  };

  const seleccionarAlumno = async (alumno) => {
    setAlumnoBusqueda(alumno.nombreCompleto || alumno.NombreCompleto);
    setAlumnosFiltrados([]);
    
    const idIdentificador = alumno.idActor || alumno.IdActor || alumno.idAlumno || alumno.IdAlumno || alumno.id || alumno.Id;

    try {
      // CORRECCIÓN: alumnos en minúscula
      const res = await fetch(`${API_URL}/alumnos/${idIdentificador}`, { headers: secureHeaders });
      if (res.ok) {
        const fullData = await res.json();
        setAlumnoSeleccionado(fullData);
        verificarBecas(idPromocion, fullData);
      } else {
        setAlumnoSeleccionado(alumno);
      }
    } catch (err) {
      console.error(err);
      setAlumnoSeleccionado(alumno);
    }
  };

  const guardarMatricula = async () => {
    if (!alumnoSeleccionado || !idPromocion || !idGrupo || !idPlanPago) {
      Alert.alert("Validación", "Por favor complete todos los campos obligatorios (*).");
      return;
    }

    setIsSubmitting(true);
    const idEstudiante = alumnoSeleccionado.idActor || alumnoSeleccionado.IdActor || alumnoSeleccionado.idalumno || alumnoSeleccionado.IdAlumno || 0;

    const payload = {
      idAlumno: Number(idEstudiante),
      idPromocion: Number(idPromocion),
      idGrupo: Number(idGrupo),
      idPlanPago: Number(idPlanPago),
      idCurso: Number(idCurso || 0),
      idBeca: Number(idBeca || 0),
      usuarioModificacion: Number(usuarioModificacion)
    };

    try {
      // CORRECCIÓN: matriculas en minúscula
      const response = await fetch(`${API_URL}/matriculas/carrera`, {
        method: 'POST',
        headers: { 
          ...secureHeaders,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData?.detail || errorData?.message || 'Ocurrió un error al procesar la matrícula.');
      }

      Alert.alert("Éxito", "Matrícula registrada correctamente.", [
        { text: "OK", onPress: () => navigation.goBack() }
      ]);
    } catch (error) {
      Alert.alert("Error de Validación", error.message);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.scrollContent} keyboardShouldPersistTaps="handled">
      
      <View style={styles.header}>
        <TouchableOpacity style={styles.backBtn} onPress={() => navigation.goBack()}>
          <Ionicons name="arrow-back" size={24} color="#334155" />
        </TouchableOpacity>
        <View style={styles.headerText}>
          <Text style={styles.title}>{isEditing ? 'Editar Matrícula' : 'Nueva Matrícula'}</Text>
          <Text style={styles.subtitle}>Configure la promoción, plan de pagos y asigne al estudiante.</Text>
        </View>
      </View>

      {/* SECCIÓN ESTUDIANTE */}
      <View style={styles.card}>
        <Text style={styles.cardHeader}>Selección de Estudiante</Text>
        <View style={styles.field}>
          <Text style={styles.label}>Buscar Alumno <Text style={styles.required}>*</Text></Text>
          <View style={styles.searchBox}>
            <TextInput
              style={styles.searchInput}
              placeholder="Ingrese apellidos o código..."
              value={alumnoBusqueda}
              onChangeText={buscarAlumno}
            />
            {isSearching ? <ActivityIndicator size="small" color="#4F46E5" /> : <Ionicons name="search" size={20} color="#94A3B8" />}
          </View>
          
          {/* Resultados del Autocomplete */}
          {alumnosFiltrados.length > 0 && (
            <View style={styles.autocompleteDropdown}>
              {alumnosFiltrados.map((item, idx) => (
                <TouchableOpacity key={idx} style={styles.autocompleteItem} onPress={() => seleccionarAlumno(item)}>
                  <Text style={styles.autocompleteText}>
                    {item.codigo || item.Codigo} - {item.nombreCompleto || item.NombreCompleto}
                  </Text>
                </TouchableOpacity>
              ))}
            </View>
          )}
        </View>

        {/* Ficha de Datos Personales */}
        {alumnoSeleccionado && (
          <View style={styles.fichaBox}>
            <Text style={styles.fichaTitle}>FICHA DE DATOS PERSONALES</Text>
            <View style={styles.fichaGrid}>
              <View style={styles.fichaItem}>
                <Text style={styles.fichaLabel}>Código</Text>
                <Text style={styles.fichaValue}>{alumnoSeleccionado.codigo || alumnoSeleccionado.Codigo || 'N/A'}</Text>
              </View>
              <View style={styles.fichaItem}>
                <Text style={styles.fichaLabel}>Doc. Identidad</Text>
                <Text style={styles.fichaValue}>
                  {alumnoSeleccionado.tipoDocumento || alumnoSeleccionado.TipoDocumento || 'DOC'} - {alumnoSeleccionado.numeroDocumento || alumnoSeleccionado.NumeroDocumento || 'N/A'}
                </Text>
              </View>
              <View style={styles.fichaItem}>
                <Text style={styles.fichaLabel}>Contacto</Text>
                <Text style={styles.fichaValue}>{alumnoSeleccionado.telefonoContacto || alumnoSeleccionado.TelefonoContacto || alumnoSeleccionado.telefono || 'N/A'}</Text>
              </View>
              <View style={styles.fichaItem}>
                <Text style={styles.fichaLabel}>E-mail</Text>
                <Text style={styles.fichaValue} numberOfLines={1}>{alumnoSeleccionado.emailPrincipal || alumnoSeleccionado.EmailPrincipal || alumnoSeleccionado.email || 'N/A'}</Text>
              </View>
            </View>
          </View>
        )}
      </View>

      {/* SECCIÓN DISTRIBUCIÓN */}
      <View style={styles.card}>
        <Text style={styles.cardHeader}>Datos de la Distribución</Text>
        
        <View style={styles.field}>
          <Text style={styles.label}>Promoción <Text style={styles.required}>*</Text></Text>
          <View style={styles.pickerBox}>
            <RNPickerSelect
              placeholder={{ label: '-- Seleccionar --', value: null }}
              value={idPromocion}
              onValueChange={setIdPromocion}
              items={promociones.map(p => ({ label: p.nombre || p.Nombre, value: p.id || p.Id }))}
              style={pickerStyles}
            />
          </View>
        </View>

        <View style={styles.field}>
          <Text style={styles.label}>Grupo <Text style={styles.required}>*</Text></Text>
          <View style={[styles.pickerBox, grupos.length === 0 && styles.disabledBox]}>
            <RNPickerSelect
              placeholder={{ label: '-- Seleccionar --', value: null }}
              value={idGrupo}
              onValueChange={setIdGrupo}
              disabled={grupos.length === 0}
              items={grupos.map(g => ({ label: g.nombre || g.Nombre, value: g.id || g.Id }))}
              style={pickerStyles}
            />
          </View>
        </View>

        <View style={styles.field}>
          <Text style={styles.label}>Curso</Text>
          <View style={[styles.pickerBox, cursos.length === 0 && styles.disabledBox]}>
            <RNPickerSelect
              placeholder={{ label: '-- Seleccionar --', value: null }}
              value={idCurso}
              onValueChange={setIdCurso}
              disabled={cursos.length === 0}
              items={cursos.map(c => ({ label: c.nombre || c.Nombre, value: c.id || c.Id }))}
              style={pickerStyles}
            />
          </View>
        </View>
      </View>

      {/* SECCIÓN FINANCIERA */}
      <View style={styles.card}>
        <Text style={styles.cardHeader}>Información Financiera</Text>
        
        <View style={styles.field}>
          <Text style={styles.label}>Plan de Pagos <Text style={styles.required}>*</Text></Text>
          <View style={[styles.pickerBox, planesPago.length === 0 && styles.disabledBox]}>
            <RNPickerSelect
              placeholder={{ label: '-- Seleccionar --', value: null }}
              value={idPlanPago}
              onValueChange={setIdPlanPago}
              disabled={planesPago.length === 0}
              items={planesPago.map(p => ({ 
                label: `${p.planNombre || p.PlanNombre} (${p.monedaNombre || p.MonedaNombre})`, 
                value: p.idPlan || p.IdPlan 
              }))}
              style={pickerStyles}
            />
          </View>
        </View>

        <View style={styles.field}>
          <Text style={styles.label}>Beca / Beneficio</Text>
          <View style={[styles.pickerBox, becas.length === 0 && styles.disabledBox]}>
            <RNPickerSelect
              placeholder={{ label: '-- Seleccionar --', value: null }}
              value={idBeca}
              onValueChange={setIdBeca}
              disabled={becas.length === 0}
              items={becas.map(b => ({ 
                label: `${b.beneficioNombre || b.BeneficioNombre} (Desc: ${b.descuento || b.Descuento}%)`, 
                value: b.idBeca || b.IdBeca 
              }))}
              style={pickerStyles}
            />
          </View>
        </View>
      </View>

      {/* BOTONES DE ACCIÓN */}
      <View style={styles.footer}>
        <TouchableOpacity style={styles.btnCancel} onPress={() => navigation.goBack()} disabled={isSubmitting}>
          <Text style={styles.btnCancelText}>Cancelar</Text>
        </TouchableOpacity>
        <TouchableOpacity style={styles.btnSave} onPress={guardarMatricula} disabled={isSubmitting}>
          {isSubmitting ? (
            <ActivityIndicator size="small" color="#FFFFFF" />
          ) : (
            <>
              <Ionicons name="checkmark-circle" size={20} color="#FFFFFF" style={{marginRight: 8}} />
              <Text style={styles.btnSaveText}>Guardar Matrícula</Text>
            </>
          )}
        </TouchableOpacity>
      </View>

    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#F8FAFC' },
  scrollContent: { padding: 15, paddingBottom: 40 },
  header: { flexDirection: 'row', alignItems: 'center', marginBottom: 20 },
  backBtn: { padding: 10, backgroundColor: '#FFFFFF', borderRadius: 8, borderWidth: 1, borderColor: '#E2E8F0', marginRight: 15 },
  headerText: { flex: 1 },
  title: { fontSize: 22, fontWeight: 'bold', color: '#0F172A' },
  subtitle: { fontSize: 13, color: '#64748B', marginTop: 2 },
  card: { backgroundColor: '#FFFFFF', borderRadius: 12, padding: 20, marginBottom: 15, borderWidth: 1, borderColor: '#E2E8F0' },
  cardHeader: { fontSize: 16, fontWeight: 'bold', color: '#1E293B', borderBottomWidth: 1, borderColor: '#F1F5F9', paddingBottom: 10, marginBottom: 15 },
  field: { marginBottom: 15, position: 'relative' },
  label: { fontSize: 13, fontWeight: '600', color: '#475569', marginBottom: 6 },
  required: { color: '#EF4444' },
  searchBox: { flexDirection: 'row', alignItems: 'center', borderWidth: 1, borderColor: '#CBD5E1', borderRadius: 8, paddingHorizontal: 12, backgroundColor: '#FFFFFF' },
  searchInput: { flex: 1, height: 44, color: '#0F172A', fontWeight: '500' },
  autocompleteDropdown: { position: 'absolute', top: 75, left: 0, right: 0, backgroundColor: '#FFFFFF', borderWidth: 1, borderColor: '#E2E8F0', borderRadius: 8, elevation: 5, shadowColor: '#000', shadowOpacity: 0.1, shadowRadius: 5, zIndex: 1000 },
  autocompleteItem: { padding: 12, borderBottomWidth: 1, borderColor: '#F1F5F9' },
  autocompleteText: { color: '#0F172A', fontSize: 14 },
  fichaBox: { backgroundColor: '#F8FAFC', padding: 15, borderRadius: 8, borderWidth: 1, borderColor: '#E2E8F0', marginTop: 10 },
  fichaTitle: { fontSize: 11, fontWeight: 'bold', color: '#94A3B8', marginBottom: 10 },
  fichaGrid: { flexDirection: 'row', flexWrap: 'wrap' },
  fichaItem: { width: '50%', marginBottom: 10 },
  fichaLabel: { fontSize: 11, color: '#64748B' },
  fichaValue: { fontSize: 13, fontWeight: '600', color: '#1E293B', marginTop: 2 },
  pickerBox: { borderWidth: 1, borderColor: '#CBD5E1', borderRadius: 8, backgroundColor: '#FFFFFF' },
  disabledBox: { backgroundColor: '#F1F5F9', borderColor: '#E2E8F0' },
  footer: { flexDirection: 'row', justifyContent: 'flex-end', alignItems: 'center', marginTop: 10 },
  btnCancel: { paddingVertical: 12, paddingHorizontal: 20, marginRight: 10 },
  btnCancelText: { color: '#64748B', fontWeight: 'bold', fontSize: 15 },
  btnSave: { backgroundColor: '#4F46E5', flexDirection: 'row', alignItems: 'center', paddingVertical: 12, paddingHorizontal: 20, borderRadius: 8 },
  btnSaveText: { color: '#FFFFFF', fontWeight: 'bold', fontSize: 15 }
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