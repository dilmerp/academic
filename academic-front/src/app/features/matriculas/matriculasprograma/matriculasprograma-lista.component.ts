import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
// IMPORT CORREGIDO: Solo 3 niveles de salto
import { MatriculaService } from '../../../core/services/matricula.service';
import { ConfirmationService, MessageService } from 'primeng/api';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { DropdownModule } from 'primeng/dropdown';

@Component({
  selector: 'app-matriculasprograma-lista',
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
  templateUrl: './matriculasprograma-lista.component.html',
  providers: [ConfirmationService, MessageService]
})
export class MatriculasProgramaListaComponent implements OnInit {
  todasLasMatriculas = signal<any[]>([]);
  promociones = signal<any[]>([]);
  promocionSeleccionada = signal<number | null>(null);

  matriculasFiltradas = computed(() => {
    const idPromocion = this.promocionSeleccionada();
    if (idPromocion === 0 || !idPromocion) {
      return this.todasLasMatriculas();
    }
    return this.todasLasMatriculas().filter(m => m.idPromocion === idPromocion);
  });

  private matriculaService = inject(MatriculaService);
  private router = inject(Router);
  private confirmationService = inject(ConfirmationService);
  private messageService = inject(MessageService);

  ngOnInit(): void {
    this.cargarPromociones();
    this.cargarTodasLasMatriculas();
  }

  cargarPromociones(): void {
    this.matriculaService.getPromociones().subscribe({
      next: (data) => {
        const opcionTodos = { id: 0, nombre: '-- Todas las Promociones --' };
        this.promociones.set([opcionTodos, ...data]);
        this.promocionSeleccionada.set(0);
      },
      error: (err) => console.error('Error al cargar las promociones', err)
    });
  }

  cargarTodasLasMatriculas(): void {
    this.matriculaService.obtenerMatriculasPrograma().subscribe({
      next: (data) => this.todasLasMatriculas.set(data),
      error: (err) => {
        console.error('Error al cargar matrículas de programa', err);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar las matrículas.' });
        this.todasLasMatriculas.set([]);
      }
    });
  }

  filtrarPorPromocion(idPromocion: number): void {
  }

  navegarNuevaMatricula(): void {
    // RUTA EXACTA ALINEADA CON APP.ROUTES.TS
    this.router.navigate(['/matriculas/programa/nueva']);
  }

  confirmarEliminacion(idRegistro: number): void {
    this.confirmationService.confirm({
      message: '¿Está seguro de que desea remover esta matrícula de programa?',
      header: 'Confirmar Eliminación',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sí, remover',
      rejectLabel: 'Cancelar',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.messageService.add({ severity: 'success', summary: 'Confirmado', detail: 'Matrícula removida exitosamente.' });
      }
    });
  }
}
