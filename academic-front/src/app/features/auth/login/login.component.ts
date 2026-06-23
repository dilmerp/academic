import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading = false;
  errorMessage = '';

  // Nuevo estado para controlar la visibilidad de la contraseña
  showPassword = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      login: ['', Validators.required],
      clave: ['']
    });
  }

  // Método para alternar el ojito
  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    // Limpieza preventiva de espacios en blanco (igual que en React Native)
    const formValue = this.loginForm.value;
    const payload = {
      ...formValue,
      login: formValue.login.trim()
    };

    this.authService.login(payload).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response && response.requiereCambioClave === true) {
          this.router.navigate(['/actualizar-clave']);
        } else {
          this.router.navigate(['/dashboard']);
        }
      },
      error: (err) => {
        this.isLoading = false;
        if (err.status === 401) {
          this.errorMessage = 'Usuario o contraseña incorrectos.';
        } else {
          // Extraer el mensaje real del backend si existe
          this.errorMessage = err.error?.message || err.error?.title || 'Error de conexión con el servidor.';
        }
      }
    });
  }
}
