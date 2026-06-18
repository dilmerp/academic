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

// Estandarización: Cero 'any'. Definimos las interfaces esperadas.
export interface CatalogoItem {
  id: number;
  nombre: string;
}

export interface AlumnoBusquedaItem {
  idActor: number;
  nombreCompleto: string;
  codigo: string;
  numeroDocumento: string;
}

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
  genero?: string | boolean;
  estadoCivil?: string;
  civilNombre?: string;
}

export interface RegistrarMatriculaProgramaPayload {
  idAlumno: number;
  idProducto: number;
  idPromocion: number;
  idGrupo: number;
  idCurso: number;
  idSeccion: number;
  idPlanPago: number;
  idBeca: number;
}

@Component({
  selector: 'app-matriculasprograma-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    ButtonModule,
    ToastModule,
    TooltipModule,
    DropdownModule,
    AutoCompleteModule
  ],
  templateUrl: './matriculasprograma-form.component.html',
  providers: [MessageService]
})
export class MatriculasProgramaFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private matriculaService = inject(MatriculaService);
  private router = inject(Router);
  private messageService = inject(MessageService);

  matriculaForm!: FormGroup;
  isSubmitting = signal<boolean>(false);

  promociones = signal<CatalogoItem[]>([]);
  grupos = signal<CatalogoItem[]>([]);
  cursos = signal<CatalogoItem[]>([]);
  alumnosFiltrados = signal<AlumnoBusquedaItem[]>([]);

  planesPago = signal<PlanPagoItem[]>([]);
  becas = signal<BecaItem[]>([]);

  alumnoSeleccionado = signal<AlumnoFicha | null>(null);

  ngOnInit(): void {
    this.initForm();
    this.cargarCatalogosMaestros();
    this.escucharCambiosPromocion();
    this.escucharCambiosGrupo();
  }

  private initForm(): void {
    this.matriculaForm = this.fb.group({
      idPromocion: [null, [Validators.required]],
      idGrupo: [{ value: null, disabled: true }, [Validators.required]],
      idCurso: [{ value: null, disabled: true }],
      alumnoObj: [null, [Validators.required]],
      idPlanPago: [{ value: null, disabled: true }, [Validators.required]],
      idBeca: [{ value: null, disabled: true }]
    });
  }

  private cargarCatalogosMaestros(): void {
    this.matriculaService.getPromociones().subscribe({
      next: (data: CatalogoItem[]) => this.promociones.set(data),
      error: (err) => console.error('Error al recuperar promociones', err)
    });
  }

  // Estandarización: Separación estricta de responsabilidades reactivas
  private escucharCambiosPromocion(): void {
    this.matriculaForm.get('idPromocion')?.valueChanges.subscribe(idPromocion => {
      const grupoCtrl = this.matriculaForm.get('idGrupo');
      const cursoCtrl = this.matriculaForm.get('idCurso');
      const planPagoCtrl = this.matriculaForm.get('idPlanPago');

      grupoCtrl?.reset();
      grupoCtrl?.disable();
      this.grupos.set([]);

      cursoCtrl?.reset();
      cursoCtrl?.disable();
      this.cursos.set([]);

      planPagoCtrl?.reset();
      planPagoCtrl?.disable();
      this.planesPago.set([]);

      this.cargarBecasSiAplica();

      if (idPromocion) {
        this.matriculaService.getGruposByPromocion(idPromocion).subscribe({
          next: (data: CatalogoItem[]) => {
            this.grupos.set(data);
            if (data && data.length > 0) grupoCtrl?.enable();
          },
          error: (err) => console.error('Error al recuperar grupos', err)
        });

        this.matriculaService.getCursosByPromocion(idPromocion).subscribe({
          next: (data: CatalogoItem[]) => {
            this.cursos.set(data);
            if (data && data.length > 0) cursoCtrl?.enable();
          },
          error: (err) => console.error('Error al recuperar cursos', err)
        });
      }
    });
  }

  private escucharCambiosGrupo(): void {
    this.matriculaForm.get('idGrupo')?.valueChanges.subscribe(idGrupo => {
      const idPromocion = this.matriculaForm.get('idPromocion')?.value;
      const planPagoCtrl = this.matriculaForm.get('idPlanPago');

      planPagoCtrl?.reset();
      planPagoCtrl?.disable();
      this.planesPago.set([]);

      if (idPromocion && idGrupo) {
        this.matriculaService.getPlanesPago(idPromocion, idGrupo, -1).subscribe({
          next: (data: PlanPagoItem[]) => {
            this.planesPago.set(data);
            if (data && data.length > 0) planPagoCtrl?.enable();
          },
          error: (err) => console.error('Error al recuperar planes de pago', err)
        });
      }
    });
  }

  private cargarBecasSiAplica(): void {
    const idActor = this.matriculaForm.get('alumnoObj')?.value?.idActor;
    const idPromocion = this.matriculaForm.get('idPromocion')?.value;
    const becaCtrl = this.matriculaForm.get('idBeca');

    becaCtrl?.reset();
    becaCtrl?.disable();
    this.becas.set([]);

    if (idActor && idPromocion) {
      this.matriculaService.getBecas(idActor, idPromocion).subscribe({
        next: (data: BecaItem[]) => {
          this.becas.set(data);
          if (data && data.length > 0) becaCtrl?.enable();
        },
        error: (err) => console.error('Error al recuperar becas', err)
      });
    }
  }

  buscarAlumno(event: AutoCompleteCompleteEvent): void {
    this.matriculaService.searchAlumnos(event.query).subscribe({
      next: (data: AlumnoBusquedaItem[]) => this.alumnosFiltrados.set(data),
      error: (err) => console.error('Error al buscar alumnos', err)
    });
  }

  seleccionarAlumno(): void {
    const alumnoObj = this.matriculaForm.get('alumnoObj')?.value;
    this.alumnoSeleccionado.set(null);

    if (alumnoObj?.idActor) {
      this.matriculaService.getAlumnoById(alumnoObj.idActor).subscribe({
        next: (data: AlumnoFicha) => {
          this.alumnoSeleccionado.set(data);
          this.cargarBecasSiAplica();
        },
        error: (err) => console.error('Error al recuperar ficha del alumno', err)
      });
    } else {
      this.matriculaForm.get('idBeca')?.reset();
      this.matriculaForm.get('idBeca')?.disable();
      this.becas.set([]);
    }
  }

  guardarMatricula(): void {
    if (this.matriculaForm.invalid) {
      this.matriculaForm.markAllAsTouched();
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Complete todos los campos obligatorios.' });
      return;
    }

    this.isSubmitting.set(true);
    const formValue = this.matriculaForm.getRawValue();

    // Estandarización: Payload plano y limpio, sin lógicas de extracción de negocio en el Front.
    const payload: RegistrarMatriculaProgramaPayload = {
      idAlumno: formValue.alumnoObj.idActor,
      idProducto: 0,
      idPromocion: formValue.idPromocion,
      idGrupo: formValue.idGrupo,
      idCurso: formValue.idCurso || 0,
      idSeccion: 0,
      idPlanPago: formValue.idPlanPago,
      idBeca: formValue.idBeca || 0
    };

    this.matriculaService.registrarMatriculaPrograma(payload).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Matrícula de programa registrada correctamente.' });
        setTimeout(() => this.router.navigate(['/matriculas/programa']), 1500);
      },
      error: (err) => {
        console.error('Error al guardar', err);
        let mensajeEspecifico = 'Ocurrió un error al procesar la matrícula.';
        if (err.error && err.error.errors && err.error.errors.length > 0) {
          mensajeEspecifico = err.error.errors[0];
        } else if (err.error?.detail || err.error?.Mensaje) {
          mensajeEspecifico = err.error.detail || err.error.Mensaje;
        }

        this.messageService.add({ severity: 'error', summary: 'Reglas de Negocio', detail: mensajeEspecifico });
        this.isSubmitting.set(false);
      }
    });
  }

  cancelar(): void {
    this.router.navigate(['/matriculas/programa']);
  }
}
