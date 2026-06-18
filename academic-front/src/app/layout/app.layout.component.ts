import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet, Router } from '@angular/router';
import { AuthService } from '../core/services/auth.service';

// Definimos la estructura para soportar submenús
interface MenuItem {
  label: string;
  icon: string;
  route?: string;
  expanded?: boolean;
  children?: MenuItem[];
}

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.layout.component.html'
})
export class AppLayoutComponent {
  // Inyectamos los servicios necesarios para la seguridad y navegación
  private authService = inject(AuthService);
  private router = inject(Router);

  menuItems: MenuItem[] = [
    { label: 'Dashboard', icon: 'pi pi-home', route: '/dashboard' },
    { label: 'Profesores', icon: 'pi pi-briefcase', route: '/profesores' },
    { label: 'Alumnos', icon: 'pi pi-users', route: '/alumnos' },
    {
      label: 'Matrículas',
      icon: 'pi pi-folder-open',
      expanded: false, // Estado inicial del acordeón (cerrado)
      children: [
        { label: 'Matrícula Carrera', icon: 'pi pi-graduation-cap', route: '/matriculas/carrera' },
        { label: 'Matrícula Programa', icon: 'pi pi-briefcase', route: '/matriculas/programa' },
        { label: 'Cursos Desaprobados', icon: 'pi pi-exclamation-triangle', route: '/matriculas/desaprobados' }
      ]
    }
  ];

  // Función para abrir/cerrar el menú desplegable
  toggleMenu(item: MenuItem): void {
    if (item.children) {
      item.expanded = !item.expanded;
    }
  }

  // Método de cierre de sesión
  logout(): void {
    this.authService.logout(); // Borra el token y los datos de sesión del localStorage
    this.router.navigate(['/login']); // Te redirige a la pantalla pública
  }
}
