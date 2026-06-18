import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ProfesorService } from '../../core/services/profesor.service';
import { Profesor } from '../../core/models/profesor.model';
import { ConfirmationService, MessageService } from 'primeng/api';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-profesores-lista',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    CardModule,
    InputTextModule,
    TooltipModule,
    ConfirmDialogModule,
    ToastModule
  ],
  templateUrl: './profesores-lista.component.html',
  providers: [ConfirmationService, MessageService]
})
export class ProfesoresListaComponent implements OnInit {
  private profesorService = inject(ProfesorService);
  private router = inject(Router);
  private confirmationService = inject(ConfirmationService);
  private messageService = inject(MessageService);

  profesores = signal<Profesor[]>([]);

  ngOnInit(): void {
    this.cargarProfesores();
  }

  cargarProfesores(): void {
    this.profesorService.getProfesores().subscribe({
      next: (data: Profesor[]) => this.profesores.set(data),
      error: (err) => console.error('Error al cargar profesores', err)
    });
  }

  irANuevoProfesor(): void {
    this.router.navigate(['/profesores/nuevo']);
  }

  // EVENTO DE EDICIÓN
  editarProfesor(id: number): void {
    if (id) {
      this.router.navigate(['/profesores/editar', id]);
    } else {
      console.error('El ID del profesor es indefinido');
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se encontró el ID del profesor.' });
    }
  }

  eliminarProfesor(profesor: any): void {
    this.confirmationService.confirm({
      message: `¿Está seguro de que desea eliminar al profesor <b>${profesor.nombreCompleto}</b>? Esta acción no se puede deshacer.`,
      header: 'Confirmar Eliminación',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sí, Eliminar',
      rejectLabel: 'Cancelar',
      acceptButtonStyleClass: 'p-button-danger',
      rejectButtonStyleClass: 'p-button-text',
      accept: () => {
        const idToDelete = profesor.id || profesor.idActor;
        this.profesorService.deleteProfesor(idToDelete).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Eliminado', detail: 'Profesor eliminado correctamente.' });
            this.cargarProfesores();
          },
          error: (err) => {
            console.error('Error al eliminar', err);
            this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo eliminar el profesor. Podría tener datos vinculados.' });
          }
        });
      }
    });
  }
}
