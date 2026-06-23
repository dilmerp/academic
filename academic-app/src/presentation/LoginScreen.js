import React, { useState } from 'react';
import { 
  View, 
  Text, 
  TextInput, 
  TouchableOpacity, 
  StyleSheet, 
  ActivityIndicator, 
  Alert,
  ImageBackground,
  KeyboardAvoidingView,
  Platform,
  ScrollView
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { authService } from '../services/authService';

export default function LoginScreen({ navigation }) {
  const [login, setLogin] = useState('');
  const [clave, setClave] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  const onSubmit = async () => {
    if (!login) {
      Alert.alert('Error', 'El campo login es requerido');
      return;
    }

    setIsLoading(true);

    try {
      const response = await authService.login({ login, clave });

      // Evaluamos la bandera de seguridad
      if (response && response.requiereCambioClave === true) {
        navigation.replace('ActualizarClave'); 
      } else {
        navigation.replace('Dashboard');
      }
    } catch (error) {
      Alert.alert('Error', error.message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <KeyboardAvoidingView 
      style={styles.mainContainer}
      behavior={Platform.OS === 'ios' ? 'padding' : undefined}
    >
      <ScrollView contentContainerStyle={styles.scrollContainer} bounces={false}>
        
        {/* Cabecera superior con imagen (Tercio de la pantalla) */}
        <ImageBackground
          source={{ uri: 'https://images.unsplash.com/photo-1541339907198-e08756dedf3f?q=80&w=800&auto=format&fit=crop' }}
          style={styles.headerBackground}
        >
          {/* Capa oscura (Overlay) similar a la web */}
          <View style={styles.overlay} />

          {/* Marca: Academic 360 (Ahora en la parte superior izquierda) */}
          <View style={styles.brandContainer}>
            <View style={styles.iconContainer}>
              <Ionicons name="school" size={32} color="#ffffff" />
            </View>
            <View style={styles.textContainer}>
              <Text style={styles.brandTitle}>Academic 360</Text>
              <Text style={styles.brandSubtitle}>Plataforma de Gestión Educativa</Text>
            </View>
          </View>
        </ImageBackground>

        {/* Contenedor del Formulario (Tarjeta superpuesta) */}
        <View style={styles.formContainer}>
          
          <View style={styles.formHeader}>
            <Text style={styles.formTitle}>Iniciar Sesión</Text>
            <Text style={styles.formSubtitle}>Ingresa tus credenciales institucionales</Text>
          </View>

          <View style={styles.inputGroup}>
            <Text style={styles.label}>Usuario Institucional</Text>
            <TextInput
              style={styles.input}
              placeholder="Ej: dpalomino"
              placeholderTextColor="#94a3b8"
              value={login}
              onChangeText={setLogin}
              autoCapitalize="none"
            />
          </View>
          
          <View style={styles.inputGroup}>
            <Text style={styles.label}>Contraseña</Text>
            <View style={styles.passwordContainer}>
              <TextInput
                style={styles.passwordInput}
                placeholder="••••••••"
                placeholderTextColor="#94a3b8"
                value={clave}
                onChangeText={setClave}
                secureTextEntry={!showPassword}
              />
              <TouchableOpacity 
                style={styles.eyeIcon} 
                onPress={() => setShowPassword(!showPassword)}
              >
                <Ionicons name={showPassword ? 'eye-off-outline' : 'eye-outline'} size={24} color="#64748b" />
              </TouchableOpacity>
            </View>
          </View>

          <TouchableOpacity style={styles.button} onPress={onSubmit} disabled={isLoading}>
            {isLoading ? (
              <ActivityIndicator color="#ffffff" />
            ) : (
              <View style={styles.buttonContent}>
                <Text style={styles.buttonText}>Ingresar al Sistema</Text>
                <Ionicons name="arrow-forward" size={20} color="#ffffff" style={{ marginLeft: 8 }} />
              </View>
            )}
          </TouchableOpacity>
          
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  mainContainer: {
    flex: 1,
    backgroundColor: '#ffffff',
  },
  scrollContainer: {
    flexGrow: 1,
  },
  headerBackground: {
    height: 320, 
    justifyContent: 'flex-start', // Alineación superior
    paddingTop: Platform.OS === 'ios' ? 70 : 60, // Espacio para la barra de estado del celular
  },
  overlay: {
    ...StyleSheet.absoluteFillObject,
    backgroundColor: 'rgba(15, 23, 42, 0.75)', 
  },
  brandContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: 25,
  },
  iconContainer: {
    width: 60,
    height: 60,
    backgroundColor: '#2563EB', 
    borderRadius: 16,
    justifyContent: 'center',
    alignItems: 'center',
    shadowColor: '#2563EB',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.4,
    shadowRadius: 8,
    elevation: 8,
  },
  textContainer: {
    marginLeft: 15,
    flex: 1,
  },
  brandTitle: {
    fontSize: 28,
    fontWeight: 'bold',
    color: '#ffffff',
    letterSpacing: -0.5,
  },
  brandSubtitle: {
    fontSize: 14,
    color: '#e2e8f0', 
    marginTop: 2,
  },
  formContainer: {
    flex: 1,
    backgroundColor: '#ffffff',
    borderTopLeftRadius: 30,
    borderTopRightRadius: 30,
    marginTop: -30, 
    paddingHorizontal: 25,
    paddingTop: 35,
    paddingBottom: 40,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: -2 },
    shadowOpacity: 0.1,
    shadowRadius: 10,
    elevation: 10,
  },
  formHeader: {
    marginBottom: 25,
  },
  formTitle: {
    fontSize: 26,
    fontWeight: 'bold',
    color: '#1e293b', 
    marginBottom: 5,
  },
  formSubtitle: {
    fontSize: 15,
    color: '#64748b', 
  },
  inputGroup: {
    marginBottom: 20,
  },
  label: {
    fontSize: 14,
    fontWeight: '600',
    color: '#334155', 
    marginBottom: 8,
  },
  input: {
    borderWidth: 1,
    borderColor: '#cbd5e1', 
    backgroundColor: '#f8fafc', 
    padding: 15,
    borderRadius: 10,
    fontSize: 16,
    color: '#1e293b',
  },
  passwordContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    borderWidth: 1,
    borderColor: '#cbd5e1',
    backgroundColor: '#f8fafc',
    borderRadius: 10,
  },
  passwordInput: {
    flex: 1,
    padding: 15,
    fontSize: 16,
    color: '#1e293b',
  },
  eyeIcon: {
    padding: 15,
  },
  button: {
    backgroundColor: '#2563EB', 
    padding: 16,
    borderRadius: 10,
    alignItems: 'center',
    justifyContent: 'center',
    marginTop: 10,
    shadowColor: '#2563EB',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 6,
    elevation: 6,
  },
  buttonContent: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
  },
  buttonText: {
    color: '#ffffff',
    fontWeight: 'bold',
    fontSize: 16,
  }
});