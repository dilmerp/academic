import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatriculaService } from '../../../core/services/matricula.service';
import { MatriculaResponse } from '../../../core/models/matricula.model';
import { ConfirmationService, MessageService } from 'primeng/api';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { DropdownModule } from 'primeng/dropdown';

@Component({
  selector: 'app-matriculascarrera-lista',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    TooltipModule,
    ConfirmDialogModule,
    ToastModule,
    DropdownModule
  ],
  templateUrl: './matriculascarrera-lista.component.html',
  providers: [ConfirmationService, MessageService]
})
export class MatriculasListaComponent implements OnInit {
  matriculas = signal<MatriculaResponse[]>([]);
  promociones = signal<{ id: number; nombre: string }[]>([]);
  promocionSeleccionada = signal<number | null>(null);
  cargando = signal<boolean>(false);

  private matriculaService = inject(MatriculaService);
  private router = inject(Router);
  private confirmationService = inject(ConfirmationService);
  private messageService = inject(MessageService);

  ngOnInit(): void {
    this.cargarPromociones();
  }

  cargarPromociones(): void {
    this.cargando.set(true);

    this.matriculaService.getPromociones().subscribe({
      next: (data: any[]) => {
        const promocionesMapeadas = (data ?? []).map(p => ({
          id: p.idpromocion || p.Idpromocion || p.IdPromocion || p.id || 0,
          nombre: p.nombre || p.Nombre || p.descripcion || p.Descripcion || 'Sin nombre'
        }));

        const opcionTodos = { id: 0, nombre: '-- Todas las Promociones --' };
        const listaFinal = [opcionTodos, ...promocionesMapeadas];

        this.promociones.set(listaFinal);
        this.promocionSeleccionada.set(null);
        this.cargando.set(false);
      },
      error: (err) => {
        console.error('Error al cargar promociones', err);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar las promociones.' });
        this.cargando.set(false);
      }
    });
  }

  onPromocionChange(idPromocion: number): void {
    if (idPromocion === null || idPromocion === undefined) return;
    this.promocionSeleccionada.set(idPromocion);
    this.cargarMatriculas(idPromocion);
  }

  cargarMatriculas(idPromocion: number): void {
    this.cargando.set(true);
    this.matriculaService.getMatriculadosByPromocion(idPromocion).subscribe({
      next: (data: MatriculaResponse[]) => {
        this.matriculas.set(data || []);
        this.cargando.set(false);
      },
      error: (err) => {
        console.error("Error cargando matrículas:", err);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar las matrículas.' });
        this.cargando.set(false);
      }
    });
  }

  navegarNuevaMatricula(): void {
    this.router.navigate(['/matriculas/carrera/nueva']);
  }

  confirmarEliminacion(idMatricula: number): void {
    this.confirmationService.confirm({
      message: '¿Está seguro de que desea remover esta matrícula?',
      header: 'Confirmar Eliminación',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sí, remover',
      rejectLabel: 'Cancelar',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.messageService.add({ severity: 'success', summary: 'Confirmado', detail: 'Matrícula removida.' });
      }
    });
  }
}
