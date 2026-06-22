import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

export const ProfesoresScreen = () => {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>Directorio de Profesores</Text>
      <Text style={styles.subtitle}>Módulo en construcción...</Text>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#F5F7FA',
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#333',
  },
  subtitle: {
    fontSize: 16,
    color: '#7F8C8D',
    marginTop: 10,
  },
});