import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ProfesorService } from '../../../core/services/profesor.service';
import { MessageService } from 'primeng/api';

import { TabViewModule } from 'primeng/tabview';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { InputNumberModule } from 'primeng/inputnumber';
import { ToastModule } from 'primeng/toast';
import { CalendarModule } from 'primeng/calendar';

@Component({
  selector: 'app-profesor-form',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, FormsModule, TabViewModule,
    InputTextModule, DropdownModule, ButtonModule, CardModule,
    TableModule, InputNumberModule, ToastModule, CalendarModule
  ],
  templateUrl: './profesor-form.component.html',
  providers: [MessageService]
})
export class ProfesorFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private profesorService = inject(ProfesorService);
  private messageService = inject(MessageService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  profesorForm!: FormGroup;

  isEditMode = false;
  profesorIdToEdit: number = 0;

  tenores = signal<any[]>([]);
  estadosCiviles = signal<any[]>([]);
  tiposDoc = signal<any[]>([]);
  listaCursosMaestro = signal<any[]>([]);

  cursosAsignados = signal<{idCurso: number | null, tarifa: number}[]>([]);

  diasSemana = ['Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado', 'Domingo'];
  horasDisponibles = signal<string[]>([
    '08:00 - 09:00', '09:00 - 10:00', '10:00 - 11:00', '11:00 - 12:00',
    '12:00 - 13:00', '13:00 - 14:00', '14:00 - 15:00', '15:00 - 16:00',
    '16:00 - 17:00', '17:00 - 18:00', '18:00 - 19:00', '19:00 - 20:00',
    '20:00 - 21:00', '21:00 - 22:00'
  ]);
  matrizDisponibilidad = signal<boolean[][]>([]);

  ngOnInit(): void {
    this.initForm();
    this.cargarMaestros();
    this.initMatrizHoraria();

    setTimeout(() => {
      this.route.paramMap.subscribe(params => {
        const idStr = params.get('id');
        if (idStr) {
          this.isEditMode = true;
          this.profesorIdToEdit = parseInt(idStr, 10);
          this.cargarDatosProfesor(this.profesorIdToEdit);
        }
      });
    }, 600);
  }

  private initForm(): void {
    this.profesorForm = this.fb.group({
      usuario: ['', Validators.required],
      paterno: ['', Validators.required],
      materno: [''],
      nombres: ['', Validators.required],
      idTenor: [null, Validators.required],
      genero: [null, Validators.required],
      idEstadoCivil: [null, Validators.required],
      emailPrincipal: ['', [Validators.email]],
      idTipoDocumento: [null],
      numeroDocumento: [''],
      fechaNacimiento: [null],
      fechaInicioDispo: [null],
      fechaFinDispo: [null]
    });
  }

  private cargarMaestros(): void {
    this.profesorService.getTenores().subscribe(data => this.tenores.set(data));
    this.profesorService.getEstadosCiviles().subscribe(data => this.estadosCiviles.set(data));
    this.profesorService.getTiposDocumento().subscribe(data => this.tiposDoc.set(data));
    this.profesorService.getCursos().subscribe(data => this.listaCursosMaestro.set(data));
  }

  agregarCurso() {
    this.cursosAsignados.update(prev => [...prev, { idCurso: null, tarifa: 0 }]);
  }

  eliminarCurso(index: number) {
    this.cursosAsignados.update(prev => prev.filter((_, i) => i !== index));
  }

  private initMatrizHoraria() {
    const filas = this.horasDisponibles().length;
    const columnas = this.diasSemana.length;
    const matrizVacia = Array(filas).fill(false).map(() => Array(columnas).fill(false));
    this.matrizDisponibilidad.set(matrizVacia);
  }

  toggleHora(horaIndex: number, diaIndex: number) {
    this.matrizDisponibilidad.update(matriz => {
      matriz[horaIndex][diaIndex] = !matriz[horaIndex][diaIndex];
      return [...matriz];
    });
  }

  volverAListado(): void {
    this.router.navigate(['/profesores']);
  }

  private convertirFechaBD(fechaStr: string): Date | null {
    if (!fechaStr) return null;
    if (fechaStr.includes('-') || fechaStr.includes('T')) return new Date(fechaStr);

    const partes = fechaStr.split('/');
    if (partes.length === 3) {
      const dia = parseInt(partes[0], 10);
      const mes = parseInt(partes[1], 10) - 1;
      const anio = parseInt(partes[2], 10);
      return new Date(anio, mes, dia);
    }
    return null;
  }

  private normalizarTexto(texto: string): string {
    if (!texto) return '';
    return texto.replace(/[^a-zA-Z0-9]/g, '').toUpperCase();
  }

  private cargarDatosProfesor(id: number): void {
    this.profesorService.getProfesorById(id).subscribe({
      next: (data: any) => {

        // =========================================================
        // DEPURACIÓN: Esto imprimirá los datos en la consola (F12)
        // =========================================================
        console.log('=== DATOS RECIBIDOS DEL BACKEND ===', data);

        const docEncontrado = this.tiposDoc().find(d =>
          d.id === data.idTipoDocumento ||
          d.id === data.idDocumento ||
          this.normalizarTexto(d.nombre) === this.normalizarTexto(data.tipoDocumento) ||
          this.normalizarTexto(d.codigo) === this.normalizarTexto(data.tipoDocumento)
        );

        const civilEncontrado = this.estadosCiviles().find(c =>
          c.id === data.idCivil ||
          c.id === data.idEstadoCivil ||
          this.normalizarTexto(c.nombre) === this.normalizarTexto(data.estadoCivil)
        );

        const tenorEncontrado = this.tenores().find(t =>
          t.id === data.idTenor ||
          this.normalizarTexto(t.nombre) === this.normalizarTexto(data.tenor) ||
          this.normalizarTexto(t.codigo) === this.normalizarTexto(data.tenor)
        );

        let generoVal = null;
        if (data.genero) {
          generoVal = this.normalizarTexto(data.genero) === 'MASCULINO';
        }

        let pat = data.apellidoPaterno || data.paterno || '';
        let mat = data.apellidoMaterno || data.materno || '';
        let nom = data.nombres || '';

        if (!pat && data.nombreCompleto) {
          const partesNombre = data.nombreCompleto.split(' ').filter((p: string) => p.trim() !== '');
          if (partesNombre.length >= 3) {
            pat = partesNombre[0];
            mat = partesNombre[1];
            nom = partesNombre.slice(2).join(' ');
          } else if (partesNombre.length === 2) {
            pat = partesNombre[0];
            nom = partesNombre[1];
          } else {
            nom = data.nombreCompleto;
          }
        }

        this.profesorForm.patchValue({
          usuario: data.usuario || '',
          paterno: pat,
          materno: mat,
          nombres: nom,
          idTenor: tenorEncontrado ? tenorEncontrado.id : null,
          genero: generoVal,
          idEstadoCivil: civilEncontrado ? civilEncontrado.id : null,
          emailPrincipal: data.emailPrincipal || data.email || '',
          idTipoDocumento: docEncontrado ? docEncontrado.id : null,
          numeroDocumento: data.numeroDocumento || data.documento || '',
          fechaNacimiento: this.convertirFechaBD(data.fechaNacimiento)
        });

        // =========================================================
        // EXTRACCIÓN AGRESIVA DE CURSOS (IGNORA MAYÚSCULAS/MINÚSCULAS)
        // =========================================================
        let cursosRaw: any[] = [];

        // 1. Validar si viene como "cursos"
        if (data.cursos && Array.isArray(data.cursos)) {
            cursosRaw = data.cursos;
        }
        // 2. Validar si viene como "Cursos"
        else if (data.Cursos && Array.isArray(data.Cursos)) {
            cursosRaw = data.Cursos;
        }
        // 3. Buscar cualquier propiedad que contenga "curso" y sea arreglo
        else {
            const keyCursos = Object.keys(data).find(k => k.toLowerCase().includes('curso') && Array.isArray(data[k]));
            if (keyCursos) cursosRaw = data[keyCursos];
        }

        // Si encontró cursos, los pinta en la tabla
        if (cursosRaw.length > 0) {
          const cursosCargados = cursosRaw.map((c: any) => ({
            idCurso: c.idCurso || c.cursoId || c.id || c.Id || null,
            tarifa: c.tarifa || c.costo || c.Tarifa || 0
          }));
          this.cursosAsignados.set(cursosCargados);
          console.log('Cursos extraídos y mapeados:', cursosCargados);
        } else {
          this.cursosAsignados.set([]);
          console.warn('Alerta: No se encontraron cursos en el JSON principal del Profesor.');
        }

        // =========================================================
        // EXTRACCIÓN AGRESIVA DE DISPONIBILIDAD
        // =========================================================
        let dispoRaw = data.disponibilidad || data.Disponibilidad || data.profesorDisponibilidad;
        if (!dispoRaw) {
            const keyDispo = Object.keys(data).find(k => k.toLowerCase().includes('disponibilidad'));
            if (keyDispo) dispoRaw = data[keyDispo];
        }

        if (dispoRaw) {
          this.profesorForm.patchValue({
            fechaInicioDispo: this.convertirFechaBD(dispoRaw.fechaInicio) || this.convertirFechaBD(dispoRaw.fechainicio) || this.convertirFechaBD(dispoRaw.FechaInicio),
            fechaFinDispo: this.convertirFechaBD(dispoRaw.fechaFin) || this.convertirFechaBD(dispoRaw.fechafin) || this.convertirFechaBD(dispoRaw.FechaFin)
          });

          let horariosRaw = dispoRaw.horarios || dispoRaw.Horarios || dispoRaw.listaHorarios || [];
          if (!horariosRaw || horariosRaw.length === 0) {
              const keyHorarios = Object.keys(dispoRaw).find(k => k.toLowerCase().includes('horario') && Array.isArray(dispoRaw[k]));
              if (keyHorarios) horariosRaw = dispoRaw[keyHorarios];
          }

          if (horariosRaw && Array.isArray(horariosRaw)) {
            const filas = this.horasDisponibles().length;
            const columnas = this.diasSemana.length;
            const matrizNueva = Array(filas).fill(false).map(() => Array(columnas).fill(false));

            horariosRaw.forEach((horario: any) => {
              const hIndex = (horario.idHora || horario.horaId || horario.idhora || horario.IdHora) - 1;
              const dIndex = (horario.idDia || horario.diaId || horario.iddia || horario.IdDia) - 1;

              if (hIndex >= 0 && hIndex < filas && dIndex >= 0 && dIndex < columnas) {
                matrizNueva[hIndex][dIndex] = true;
              }
            });
            this.matrizDisponibilidad.set(matrizNueva);
          }
        }
      },
      error: (err) => {
        console.error('Error al cargar los datos del profesor', err);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo cargar la información del profesor.' });
      }
    });
  }

  onSubmit(): void {
    if (this.profesorForm.valid) {
      const formValues = this.profesorForm.value;

      const horariosSeleccionados = [];
      const matriz = this.matrizDisponibilidad();

      for (let h = 0; h < matriz.length; h++) {
        for (let d = 0; d < matriz[h].length; d++) {
          if (matriz[h][d]) {
             horariosSeleccionados.push({ idHora: h + 1, idDia: d + 1 });
          }
        }
      }

      const cursosValidos = this.cursosAsignados().filter(c => c.idCurso !== null);

      const payload = {
        idActor: this.isEditMode ? this.profesorIdToEdit : 0,
        usuario: formValues.usuario,
        contrasena: "123456",
        apellidoPaterno: formValues.paterno,
        apellidoMaterno: formValues.materno || "",
        nombres: formValues.nombres,
        idTenor: formValues.idTenor,
        genero: formValues.genero,
        idCivil: formValues.idEstadoCivil,
        email: formValues.emailPrincipal || "",
        emailOpcional: "",
        emailAdicional: "",
        fechaNacimiento: formValues.fechaNacimiento ? new Date(formValues.fechaNacimiento).toISOString() : new Date().toISOString(),
        idPaisNacimiento: 1,
        idUbigeoNacimiento: "",
        idUbigeoNacimientoOtro: "",
        telefono: "",
        celular: "",
        telefonoReferencial: "",
        direccion: "",
        urbanizacion: "",
        direccionReferencia: "",
        idPais: 1,
        idUbigeo: "",
        idUbigeoResidenciaOtro: "",
        idNacionalidad: 1,
        codigoPostal: "",
        idDocumento: formValues.idTipoDocumento || 1,
        numeroDocumento: formValues.numeroDocumento || "",
        ruc: "",
        estado: "A",
        idPeriodo: 1,
        usuarioCreacionId: 1,

        cursos: cursosValidos,
        disponibilidad: {
          fechaInicio: formValues.fechaInicioDispo ? new Date(formValues.fechaInicioDispo).toISOString() : new Date().toISOString(),
          fechaFin: formValues.fechaFinDispo ? new Date(formValues.fechaFinDispo).toISOString() : new Date().toISOString(),
          horarios: horariosSeleccionados
        }
      };

      if (this.isEditMode) {
        this.profesorService.updateProfesor(this.profesorIdToEdit, payload).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Profesor actualizado correctamente.' });
            setTimeout(() => this.volverAListado(), 1500);
          },
          error: (err) => {
            console.error('Error al actualizar el profesor', err);
            this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Ocurrió un problema al actualizar.' });
          }
        });
      } else {
        this.profesorService.createProfesor(payload).subscribe({
          next: (response: any) => {
            const codigoGenerado = response.codigo || response.Codigo || 'Generado con éxito';
            this.messageService.add({ severity: 'success', summary: `Código: ${codigoGenerado}`, detail: 'Profesor registrado con sus cursos y horarios correctamente.' });
            setTimeout(() => this.volverAListado(), 3000);
          },
          error: (err) => {
            console.error('Error al grabar el profesor', err);
            this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Ocurrió un problema al guardar.' });
          }
        });
      }
    } else {
      this.messageService.add({ severity: 'warn', summary: 'Validación', detail: 'Por favor complete los campos obligatorios.' });
      this.profesorForm.markAllAsTouched();
    }
  }
}
