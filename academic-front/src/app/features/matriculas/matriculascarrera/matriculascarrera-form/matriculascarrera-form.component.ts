import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MatriculaService } from '../../../../core/services/matricula.service';
import { MessageService } from 'primeng/api';
import { PlanPagoItem, BecaItem } from '../../../../core/models/matricula.model';

import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { DropdownModule } from 'primeng/dropdown';
import { AutoCompleteCompleteEvent, AutoCompleteModule } from 'primeng/autocomplete';

export interface CatalogoItem { id: number; nombre: string; }
export interface AlumnoBusquedaItem { idActor: number; nombreCompleto: string; codigo: string; }

export interface AlumnoFicha {
  codigo?: string;
  tipoDocumento?: string;
  documentoCodigo?: string;
  numeroDocumento?: string;
  telefonoContacto?: string;
  telefono?: string;
  emailPrincipal?: string;
  eMail?: string;
  email?: string;
  direccionCompleta?: string;
  direccion?: string;
  ubicacionResidencia?: string;
  distritoNombreResidencia?: string;
  genero?: any;
  estadoCivil?: string;
  civilNombre?: string;
}

@Component({
  selector: 'app-matriculascarrera-form',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, RouterModule, ButtonModule,
    ToastModule, TooltipModule, DropdownModule, AutoCompleteModule
  ],
  templateUrl: './matriculascarrera-form.component.html',
  providers: [MessageService]
})
export class MatriculasFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private matriculaService = inject(MatriculaService);
  private router = inject(Router);
  private messageService = inject(MessageService);

  matriculaForm: FormGroup = this.fb.group({
    idPromocion: [null, Validators.required],
    idGrupo: [{ value: null, disabled: true }, Validators.required],
    idPlanPago: [{ value: null, disabled: true }, Validators.required],
    idCurso: [{ value: null, disabled: true }],
    alumnoObj: [null, Validators.required],
    idBeca: [{ value: null, disabled: true }],
    usuarioModificacion: [1, Validators.required]
  });

  isSubmitting = signal<boolean>(false);
  promociones = signal<CatalogoItem[]>([]);
  grupos = signal<CatalogoItem[]>([]);
  cursos = signal<CatalogoItem[]>([]);
  planesPago = signal<PlanPagoItem[]>([]);
  becas = signal<BecaItem[]>([]);
  alumnosFiltrados = signal<AlumnoBusquedaItem[]>([]);
  alumnoSeleccionado = signal<AlumnoFicha | null>(null);

  ngOnInit(): void {
    this.cargarCatalogosMaestros();
    this.escucharCambiosPromocion();
    this.escucharCambiosGrupo();
  }

  private cargarCatalogosMaestros(): void {
    this.matriculaService.getPromociones().subscribe({
      next: (data) => this.promociones.set(data),
      error: (err) => console.error(err)
    });
  }

  private escucharCambiosPromocion(): void {
    this.matriculaForm.get('idPromocion')?.valueChanges.subscribe(id => {
      this.resetDependentControls(['idGrupo', 'idPlanPago', 'idCurso']);
      if (!id) return;

      this.matriculaService.getGruposByPromocion(id).subscribe(data => {
        this.grupos.set(data);
        if (data.length > 0) this.matriculaForm.get('idGrupo')?.enable();
      });
      this.matriculaService.getCursosByPromocion(id).subscribe(data => {
        this.cursos.set(data);
        if (data.length > 0) this.matriculaForm.get('idCurso')?.enable();
      });
      this.verificarBecas();
    });
  }

  private escucharCambiosGrupo(): void {
    this.matriculaForm.get('idGrupo')?.valueChanges.subscribe(idGrupo => {
      const idPromocion = this.matriculaForm.get('idPromocion')?.value;
      this.matriculaForm.get('idPlanPago')?.reset();
      if (idPromocion && idGrupo) {
        this.matriculaService.getPlanesPago(idPromocion, idGrupo, -1).subscribe(data => {
          this.planesPago.set(data);
          if (data.length > 0) this.matriculaForm.get('idPlanPago')?.enable();
        });
      }
    });
  }

  private resetDependentControls(names: string[]): void {
    names.forEach(n => {
      this.matriculaForm.get(n)?.reset();
      this.matriculaForm.get(n)?.disable();
    });
  }

  private verificarBecas(): void {
    const alumno = this.matriculaForm.get('alumnoObj')?.value;
    const promo = this.matriculaForm.get('idPromocion')?.value;
    if (alumno?.idActor && promo) {
      this.matriculaService.getBecas(alumno.idActor, promo).subscribe(data => {
        this.becas.set(data);
        if (data.length > 0) this.matriculaForm.get('idBeca')?.enable();
      });
    }
  }

  buscarAlumno(event: AutoCompleteCompleteEvent): void {
    this.matriculaService.searchAlumnos(event.query).subscribe(data => this.alumnosFiltrados.set(data));
  }

  seleccionarAlumno(): void {
    const alumnoObj = this.matriculaForm.get('alumnoObj')?.value;
    if (alumnoObj?.idActor) {
      this.matriculaService.getAlumnoById(alumnoObj.idActor).subscribe(data => {
        this.alumnoSeleccionado.set(data);
        this.verificarBecas();
      });
    } else {
      this.alumnoSeleccionado.set(null);
    }
  }

  guardarMatricula(): void {
    if (this.matriculaForm.invalid) {
      this.matriculaForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    const raw = this.matriculaForm.getRawValue();

    const payload = {
      idAlumno: Number(raw.alumnoObj?.idActor || 0),
      idPromocion: Number(raw.idPromocion || 0),
      idGrupo: Number(raw.idGrupo || 0),
      idPlanPago: Number(raw.idPlanPago || 0),
      idCurso: Number(raw.idCurso || 0),
      idBeca: Number(raw.idBeca || 0),
      usuarioModificacion: Number(raw.usuarioModificacion || 1)
    };

    this.matriculaService.registrarMatriculaCarrera(payload as unknown as any).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Matrícula registrada correctamente.' });
        setTimeout(() => this.router.navigate(['/matriculas']), 1500);
      },
      error: (err) => {
        console.error('Error capturado de la API:', err);

        // Extracción dinámica del mensaje detallado devuelto por el controlador
        const mensajeError = err.error?.detail || 'Ocurrió un error inesperado al procesar la matrícula.';

        this.messageService.add({
          severity: 'error',
          summary: 'Validación de Matrícula',
          detail: mensajeError
        });

        this.isSubmitting.set(false);
      }
    });
  }

  cancelar(): void { this.router.navigate(['/matriculas']); }
}
