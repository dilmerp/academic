import { Component,OnInit,inject,signal} from "@angular/core";
import { CommonModule } from "@angular/common";
import { Router } from "@angular/router";
import { AlumnoService } from "../../core/services/alumno.service";
import { Alumno } from "../../core/models/alumno.model";
import { ConfirmationService, MessageService } from 'primeng/api';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';



@Component({
  selector: 'app-alumnos-lista',
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
  templateUrl: './alumnos-lista.component.html',
  providers: [ConfirmationService, MessageService]
})

export class AlumnosListaComponent implements OnInit {
  alumnos = signal<any[]>([]);
  private alumnoService = inject(AlumnoService);
  private router = inject(Router);
  private confirmationService = inject(ConfirmationService);
  private messageService = inject(MessageService);

  profesores = signal<Alumno[]>([]);

  ngOnInit(): void {
    this.alumnoService.getAlumnos().subscribe({
      next: (data) => {
        this.alumnos.set(data);
      }
    });
  }

  cargarAlumnos(): void {
    this.alumnoService.getAlumnos().subscribe({
      next: (data: Alumno[]) => this.alumnos.set(data),
      error: (err) => console.error('Error al cargar alumnos', err)
    });
  }
}

