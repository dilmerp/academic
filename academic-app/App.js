import React, { useState } from 'react';
import { 
  View, 
  Text, 
  TouchableOpacity, 
  StyleSheet, 
  SafeAreaView, 
  StatusBar, 
  Modal, 
  TouchableWithoutFeedback,
  ImageBackground,
  Image
} from 'react-native';
import { NavigationContainer, useNavigationContainerRef } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { Ionicons } from '@expo/vector-icons';

// --- SEGURIDAD Y AUTENTICACIÓN ---
import LoginScreen from './src/presentation/LoginScreen';
import ActualizarClaveScreen from './src/presentation/ActualizarClaveScreen';

// --- COMPONENTES PRINCIPALES ---
import { DashboardScreen } from './src/presentation/DashboardScreen';
import { ProfesoresScreen } from './src/presentation/ProfesoresScreen';
import { AlumnosScreen } from './src/presentation/AlumnosScreen';

// --- COMPONENTES PARA MATRÍCULAS ---
import { MatriculaCarreraListScreen } from './src/presentation/MatriculaCarreraListScreen';
import { MatriculaCarreraFormScreen } from './src/presentation/MatriculaCarreraFormScreen';

// (Opcional) Importamos el authService si deseas limpiar el SecureStore al cerrar sesión directamente desde aquí
import { authService } from './src/services/authService';

const Stack = createNativeStackNavigator();

// Stack Navigator específico para el flujo de Matrículas
const MatriculasStack = () => (
  <Stack.Navigator screenOptions={{ headerShown: false }}>
    <Stack.Screen name="MatriculaCarreraList" component={MatriculaCarreraListScreen} />
    <Stack.Screen name="MatriculaCarreraForm" component={MatriculaCarreraFormScreen} />
  </Stack.Navigator>
);

const MenuLateral = ({ visible, onClose, navigateTo, currentRoute, handleLogout }) => {
  // Estado para controlar qué submenú está abierto
  const [activeSubMenu, setActiveSubMenu] = useState(null);

  const DrawerItem = ({ iconName, label, routeName, hasSubMenu, subMenuRoute, subMenuItems = [] }) => {
    const isActive = currentRoute === routeName;
    const isSubMenuOpen = activeSubMenu === routeName;

    return (
      <View>
        <TouchableOpacity 
          style={[styles.menuItem, isActive && styles.menuItemActive, hasSubMenu && styles.menuItemWithSubMenu]} 
          onPress={() => {
            if (hasSubMenu) {
              setActiveSubMenu(isSubMenuOpen ? null : routeName);
            } else {
              navigateTo(routeName);
            }
          }}
          activeOpacity={0.7}
        >
          <View style={styles.menuItemLeftContent}>
            <Ionicons 
              name={iconName} 
              size={22} 
              color={isActive ? '#3B82F6' : '#94A3B8'} 
              style={styles.menuIcon} 
            />
            <Text style={[styles.menuText, isActive && styles.menuTextActive]}>
              {label}
            </Text>
          </View>
          {hasSubMenu && (
            <Ionicons 
              name={isSubMenuOpen ? 'chevron-up' : 'chevron-down'} 
              size={18} 
              color="#94A3B8" 
            />
          )}
        </TouchableOpacity>
        
        {/* Renderiza los ítems del submenú si está abierto */}
        {hasSubMenu && isSubMenuOpen && (
          <View style={styles.subMenuItemsContainer}>
            {subMenuItems.map((item, index) => {
              const isSubItemActive = currentRoute === item.routeName;
              return (
                <TouchableOpacity 
                  key={index}
                  style={[styles.subMenuItem, isSubItemActive && styles.subMenuItemActive]}
                  onPress={() => navigateTo(item.routeName)}
                  activeOpacity={0.7}
                >
                  <Ionicons 
                    name={item.iconName} 
                    size={20} 
                    color={isSubItemActive ? '#FFFFFF' : '#94A3B8'} 
                    style={styles.subMenuIcon} 
                  />
                  <Text style={[styles.subMenuText, isSubItemActive && styles.subMenuTextActive]}>
                    {item.label}
                  </Text>
                </TouchableOpacity>
              );
            })}
          </View>
        )}
      </View>
    );
  };

  return (
    <Modal visible={visible} transparent={true} animationType="fade" onRequestClose={onClose}>
      <View style={styles.modalOverlay}>
        <TouchableWithoutFeedback onPress={onClose}>
          <View style={styles.modalBackground} />
        </TouchableWithoutFeedback>

        <View style={styles.drawerContainer}>
          <ImageBackground source={{ uri: 'https://images.unsplash.com/photo-1541339907198-e08756dedf3f?q=80&w=600&auto=format&fit=crop' }} style={styles.drawerHeader} imageStyle={{ opacity: 0.4 }}>
            <Image source={{ uri: 'https://randomuser.me/api/portraits/men/32.jpg' }} style={styles.avatar} />
            <Text style={styles.userName}>Administrador</Text>
            <Text style={styles.userRole}>Gestión Académica</Text>
          </ImageBackground>

          <View style={styles.menuItemsContainer}>
            <DrawerItem iconName="grid-outline" label="Panel de Inicio" routeName="Dashboard" />
            <DrawerItem iconName="school-outline" label="Docentes" routeName="Profesores" />
            <DrawerItem iconName="people-outline" label="Alumnos" routeName="Alumnos" />
            
            {/* NUEVO SUBMENÚ DE MATRÍCULAS */}
            <DrawerItem 
              iconName="book-outline" 
              label="Matrículas" 
              routeName="Matriculas" 
              hasSubMenu={true} 
              subMenuItems={[
                { label: 'Carrera', iconName: 'arrow-forward-outline', routeName: 'Matriculas' },
                { label: 'Programa y Cursos', iconName: 'time-outline', routeName: 'Dashboard' }, // Falsa ruta por ahora
              ]}
            />
          </View>

          <View style={styles.drawerFooter}>
            <TouchableOpacity style={styles.logoutButton} onPress={handleLogout}>
              <Ionicons name="log-out-outline" size={22} color="#EF4444" />
              <Text style={styles.logoutText}>Cerrar Sesión</Text>
            </TouchableOpacity>
            <Text style={styles.versionText}>AtlanticApp v1.0.0</Text>
          </View>
        </View>
      </View>
    </Modal>
  );
};

export default function App() {
  const [menuVisible, setMenuVisible] = useState(false);
  const [currentRoute, setCurrentRoute] = useState('Login');
  const navigationRef = useNavigationContainerRef();

  const handleNavigate = (screenName) => {
    setCurrentRoute(screenName);
    setMenuVisible(false);
    if (navigationRef.isReady()) {
      navigationRef.navigate(screenName);
    }
  };

  const executeLogout = async () => {
    try {
      await authService.logout(); // Limpia la bóveda de seguridad
    } catch (e) {
      console.log('Error limpiando sesión', e);
    }
    setMenuVisible(false);
    if (navigationRef.isReady()) {
      // Limpia el historial de navegación y te deja en la pantalla de Login
      navigationRef.reset({
        index: 0,
        routes: [{ name: 'Login' }],
      });
    }
  };

  const CustomHeaderLeft = () => (
    <TouchableOpacity style={styles.hamburgerButton} onPress={() => setMenuVisible(true)}>
      <Ionicons name="menu-outline" size={28} color="#0F172A" />
    </TouchableOpacity>
  );

  return (
    <SafeAreaView style={styles.safeArea}>
      <StatusBar barStyle="dark-content" backgroundColor="#FFFFFF" />
      <NavigationContainer ref={navigationRef} onStateChange={() => {
        const current = navigationRef.getCurrentRoute()?.name;
        if (current) setCurrentRoute(current);
      }}>
        <Stack.Navigator
          initialRouteName="Login"
          screenOptions={{
            headerLeft: () => <CustomHeaderLeft />,
            headerStyle: { backgroundColor: '#FFFFFF', borderBottomWidth: 1, borderBottomColor: '#F1F5F9' },
            headerTintColor: '#0F172A',
            headerTitleStyle: { fontWeight: '700', fontSize: 18 },
            headerTitleAlign: 'center',
          }}
        >
          {/* --- FLUJO DE AUTENTICACIÓN --- */}
          <Stack.Screen 
            name="Login" 
            component={LoginScreen} 
            options={{ headerShown: false }} 
          />
          <Stack.Screen 
            name="ActualizarClave" 
            component={ActualizarClaveScreen} 
            options={{ 
              title: 'Actualizar Credenciales', 
              headerLeft: () => null // Sobrescribe la hamburguesa para no dejar escapar al usuario
            }} 
          />

          {/* --- FLUJO INTERNO DEL SISTEMA --- */}
          <Stack.Screen name="Dashboard" component={DashboardScreen} options={{ title: 'Inicio' }} />
          <Stack.Screen name="Profesores" component={ProfesoresScreen} options={{ title: 'Docentes' }} />
          <Stack.Screen name="Alumnos" component={AlumnosScreen} options={{ title: 'Alumnos' }} />
          
          {/* STACK DE MATRÍCULAS QUE SE MUESTRA EN EL HEADER COMO UN SOLO TÍTULO */}
          <Stack.Screen name="Matriculas" component={MatriculasStack} options={{ title: 'Matrículas de Carreras' }} />
        </Stack.Navigator>
      </NavigationContainer>

      {/* El menú lateral solo se renderiza por encima de todo cuando menuVisible es true */}
      <MenuLateral 
        visible={menuVisible} 
        onClose={() => setMenuVisible(false)} 
        navigateTo={handleNavigate} 
        currentRoute={currentRoute} 
        handleLogout={executeLogout}
      />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safeArea: { flex: 1, backgroundColor: '#F8FAFC' },
  hamburgerButton: { marginLeft: 10, padding: 5 },
  modalOverlay: { flex: 1, flexDirection: 'row' },
  modalBackground: { ...StyleSheet.absoluteFillObject, backgroundColor: 'rgba(15, 23, 42, 0.65)' },
  drawerContainer: { width: 290, height: '100%', backgroundColor: '#0B1120', elevation: 10, display: 'flex', justifyContent: 'space-between' },
  drawerHeader: { paddingTop: 60, paddingBottom: 30, paddingHorizontal: 20, backgroundColor: '#1E293B' },
  avatar: { width: 70, height: 70, borderRadius: 35, borderWidth: 2, borderColor: '#3B82F6', marginBottom: 15 },
  userName: { color: '#F8FAFC', fontSize: 20, fontWeight: 'bold' },
  userRole: { color: '#94A3B8', fontSize: 14, marginTop: 2 },
  menuItemsContainer: { flex: 1, paddingTop: 20 },
  menuItem: { flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between', paddingVertical: 14, paddingHorizontal: 25, marginHorizontal: 10, borderRadius: 8, marginBottom: 5 },
  menuItemLeftContent: { flexDirection: 'row', alignItems: 'center' },
  menuItemActive: { backgroundColor: 'rgba(59, 130, 246, 0.15)' },
  menuIcon: { marginRight: 15 },
  menuText: { color: '#94A3B8', fontSize: 16, fontWeight: '500' },
  menuTextActive: { color: '#3B82F6', fontWeight: 'bold' },
  drawerFooter: { padding: 20, borderTopWidth: 1, borderTopColor: '#1E293B' },
  logoutButton: { flexDirection: 'row', alignItems: 'center', marginBottom: 15 },
  logoutText: { color: '#EF4444', fontSize: 16, fontWeight: 'bold', marginLeft: 10 },
  versionText: { color: '#475569', fontSize: 12, textAlign: 'center' },
  
  // --- ESTILOS DE SUBMENÚ ---
  subMenuItemsContainer: { paddingLeft: 45, marginBottom: 10 },
  subMenuItem: { flexDirection: 'row', alignItems: 'center', paddingVertical: 10, borderRadius: 6 },
  subMenuItemActive: { backgroundColor: '#1E293B' }, // Fondo oscuro sutil para el ítem activo
  subMenuIcon: { marginRight: 12 },
  subMenuText: { color: '#94A3B8', fontSize: 14, fontWeight: '500' },
  subMenuTextActive: { color: '#FFFFFF', fontWeight: 'bold' },
});