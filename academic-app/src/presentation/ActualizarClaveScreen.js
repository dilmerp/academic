import React, { useState, useEffect } from 'react';
import { View, Text, TextInput, TouchableOpacity, StyleSheet, ActivityIndicator, Alert } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import * as SecureStore from 'expo-secure-store';
import { authService } from '../services/authService';

export default function ActualizarClaveScreen({ navigation }) {
  const [nuevaClave, setNuevaClave] = useState('');
  const [confirmarClave, setConfirmarClave] = useState('');
  const [loginUser, setLoginUser] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  // Controles de visibilidad independientes para cada input
  const [showNuevaClave, setShowNuevaClave] = useState(false);
  const [showConfirmarClave, setShowConfirmarClave] = useState(false);

  useEffect(() => {
    // Recupera el login que guardó el authService
    const fetchUser = async () => {
      const user = await SecureStore.getItemAsync('login');
      if (!user) {
        navigation.replace('Login'); // Protección de ruta
      } else {
        setLoginUser(user);
      }
    };
    fetchUser();
  }, []);

  const onSubmit = async () => {
    if (nuevaClave.length < 6) {
      Alert.alert('Error', 'La contraseña debe tener al menos 6 caracteres');
      return;
    }
    
    if (nuevaClave !== confirmarClave) {
      Alert.alert('Error', 'Las contraseñas no coinciden');
      return;
    }

    setIsLoading(true);

    try {
      await authService.actualizarClave({ login: loginUser, nuevaClave });
      
      // Cambio exitoso, entra directo al sistema
      Alert.alert('Éxito', 'Credenciales actualizadas correctamente');
      navigation.replace('Dashboard');
    } catch (error) {
      Alert.alert('Error', error.message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Actualizar Contraseña</Text>
      <Text style={styles.subtitle}>
        Por políticas de protección institucional, es obligatorio cambiar tu contraseña en este primer inicio de sesión.
      </Text>

      <View style={styles.passwordContainer}>
        <TextInput
          style={styles.passwordInput}
          placeholder="Introduce tu nueva contraseña"
          value={nuevaClave}
          onChangeText={setNuevaClave}
          secureTextEntry={!showNuevaClave}
        />
        <TouchableOpacity 
          style={styles.eyeIcon} 
          onPress={() => setShowNuevaClave(!showNuevaClave)}
        >
          <Ionicons name={showNuevaClave ? 'eye-outline' : 'eye-off-outline'} size={24} color="#666" />
        </TouchableOpacity>
      </View>

      <View style={styles.passwordContainer}>
        <TextInput
          style={styles.passwordInput}
          placeholder="Repite tu nueva contraseña"
          value={confirmarClave}
          onChangeText={setConfirmarClave}
          secureTextEntry={!showConfirmarClave}
        />
        <TouchableOpacity 
          style={styles.eyeIcon} 
          onPress={() => setShowConfirmarClave(!showConfirmarClave)}
        >
          <Ionicons name={showConfirmarClave ? 'eye-outline' : 'eye-off-outline'} size={24} color="#666" />
        </TouchableOpacity>
      </View>

      <TouchableOpacity style={styles.button} onPress={onSubmit} disabled={isLoading}>
        {isLoading ? <ActivityIndicator color="#fff" /> : <Text style={styles.buttonText}>Actualizar Credenciales</Text>}
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, padding: 20, justifyContent: 'center', backgroundColor: '#fff' },
  title: { fontSize: 24, fontWeight: 'bold', marginBottom: 10, color: '#003366' },
  subtitle: { fontSize: 14, color: '#666', marginBottom: 25, lineHeight: 20 },
  passwordContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    borderWidth: 1,
    borderColor: '#ccc',
    borderRadius: 8,
    marginBottom: 15,
    backgroundColor: '#fff',
  },
  passwordInput: {
    flex: 1,
    padding: 15,
  },
  eyeIcon: {
    padding: 15,
  },
  button: { backgroundColor: '#3385ff', padding: 15, borderRadius: 8, alignItems: 'center', marginTop: 10 },
  buttonText: { color: '#fff', fontWeight: 'bold', fontSize: 16 }
});