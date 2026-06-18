import { Routes } from '@angular/router';
import { Component } from '@angular/core';

// Importamos el Layout y los Componentes de Profesores
import { AppLayoutComponent } from './layout/app.layout.component';
import { ProfesoresListaComponent } from './features/profesores/profesores-lista.component';
import { ProfesorFormComponent } from './features/profesores/profesor-form/profesor-form.component';
import { AlumnosListaComponent } from './features/alumnos/alumnos-lista.component';

// IMPORTACIÓN DEL MÓDULO DE MATRÍCULAS
import { MatriculasListaComponent } from './features/matriculas/matriculascarrera/matriculascarrera-lista.component';
import { MatriculasFormComponent } from './features/matriculas/matriculascarrera/matriculascarrera-form/matriculascarrera-form.component';

import { MatriculasProgramaListaComponent } from './features/matriculas/matriculasprograma/matriculasprograma-lista.component';
import { MatriculasProgramaFormComponent } from './features/matriculas/matriculasprograma/matriculasprograma-form/matriculasprograma-form.component';

// IMPORTACIÓN DE COMPONENTES DE AUTENTICACIÓN Y SEGURIDAD
import { LoginComponent } from './features/auth/login/login.component';
import { ActualizarClaveComponent } from './features/auth/actualizar-clave/actualizar-clave.component';
import { authGuard } from './core/guards/auth.guard';


// Componente en línea para el Dashboard
@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `
    <div class="flex items-center justify-center h-full min-h-[50vh] text-slate-500">
      <div class="text-center bg-white p-8 rounded-xl shadow-sm border border-slate-100">
        <i class="pi pi-compass text-6xl mb-4 text-blue-400"></i>
        <h2 class="text-2xl font-semibold m-0 text-slate-700">Bienvenido a Academic API</h2>
        <p class="mt-2 text-slate-500">Seleccione una opción del menú lateral para comenzar a trabajar.</p>
      </div>
    </div>
  `
})
export class DashboardComponent {}

export const routes: Routes = [
  // 1. Rutas públicas de Autenticación y Seguridad (Fuera del contenedor del Layout lateral)
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'actualizar-clave',
    component: ActualizarClaveComponent
  },

  // 2. Rutas del Sistema Protegidas (Envueltas por el Layout y el Guardián de seguridad)
  {
    path: '',
    component: AppLayoutComponent,
    canActivate: [authGuard], // El guardián intercepta y protege automáticamente todas las rutas hijas
    children: [
      // Redirección inicial hacia el dashboard
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },

      // Pantalla de inicio
      { path: 'dashboard', component: DashboardComponent },

      // Pantallas de Profesores
      { path: 'profesores', component: ProfesoresListaComponent },
      { path: 'profesores/nuevo', component: ProfesorFormComponent },
      { path: 'profesores/editar/:id', component: ProfesorFormComponent },

      // Pantallas de Alumnos
      { path: 'alumnos', component: AlumnosListaComponent },

      // PANTALLAS DE MATRÍCULAS (ESTRUCTURA MODULAR SRP)

      // 1. Redirección base del módulo
      { path: 'matriculas', redirectTo: 'matriculas/carrera', pathMatch: 'full' },

      // 2. Flujo de Matrícula en Carrera
      { path: 'matriculas/carrera', component: MatriculasListaComponent },
      { path: 'matriculas/carrera/nueva', component: MatriculasFormComponent },

      // 3. Flujo de Matrícula en Programa
      { path: 'matriculas/programa', component: MatriculasProgramaListaComponent },
      { path: 'matriculas/programa/nueva', component: MatriculasProgramaFormComponent }
    ]
  },

  // Comodín: Cualquier ruta no definida o inválida redirige automáticamente al login
  { path: '**', redirectTo: 'login' }
];
