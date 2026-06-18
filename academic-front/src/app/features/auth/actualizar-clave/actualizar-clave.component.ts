import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-actualizar-clave',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './../../../features/auth/actualizar-clave/actualizar-clave.component.html',
  styleUrls: ['./../../../features/auth/actualizar-clave/actualizar-clave.component.css']
})
export class ActualizarClaveComponent implements OnInit {
  actualizarClaveForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  loginUser = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.actualizarClaveForm = this.fb.group({
      nuevaClave: ['', [Validators.required, Validators.minLength(6)]],
      confirmarClave: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit(): void {
    // Recupera el login que guardó el AuthService de forma automática en el paso anterior
    this.loginUser = localStorage.getItem('login') || '';

    // Protección de ruta: Si no hay un usuario registrado en el flujo, se expulsa al login
    if (!this.loginUser) {
      this.router.navigate(['/auth/login']);
    }
  }

  passwordMatchValidator(form: FormGroup) {
    const nueva = form.get('nuevaClave')?.value;
    const confirmar = form.get('confirmarClave')?.value;
    return nueva === confirmar ? null : { mismatch: true };
  }

  onSubmit(): void {
    if (this.actualizarClaveForm.invalid) {
      this.actualizarClaveForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const payload = {
      login: this.loginUser,
      nuevaClave: this.actualizarClaveForm.value.nuevaClave
    };

    this.authService.actualizarClave(payload).subscribe({
      next: () => {
        this.isLoading = false;
        // Una vez cambiada con éxito en la BD, la bandera pasa a false y entra directo al sistema
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Hubo un problema al procesar el cambio de clave institucional.';
        console.error(err);
      }
    });
  }
}
