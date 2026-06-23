import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-actualizar-clave',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './actualizar-clave.component.html',
  styleUrls: ['./actualizar-clave.component.css']
})
export class ActualizarClaveComponent implements OnInit {
  actualizarClaveForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  loginUser = '';

  // Controles independientes para los ojitos
  showNuevaClave = false;
  showConfirmarClave = false;

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
    this.loginUser = localStorage.getItem('login') || '';
    if (!this.loginUser) {
      this.router.navigate(['/auth/login']);
    }
  }

  passwordMatchValidator(form: FormGroup) {
    const nueva = form.get('nuevaClave')?.value;
    const confirmar = form.get('confirmarClave')?.value;
    return nueva === confirmar ? null : { mismatch: true };
  }

  // Métodos para alternar visibilidad
  toggleNuevaClave(): void {
    this.showNuevaClave = !this.showNuevaClave;
  }

  toggleConfirmarClave(): void {
    this.showConfirmarClave = !this.showConfirmarClave;
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
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Hubo un problema al procesar el cambio de clave institucional.';
      }
    });
  }
}
