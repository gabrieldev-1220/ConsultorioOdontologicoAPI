using ConsultorioOdontologicoAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConsultorioOdontologicoAPI.Data
{
    public class ConsultorioOdontologicoContext : DbContext
    {
        public ConsultorioOdontologicoContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Odontologo> Odontologos { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<HistorialClinico> HistorialClinico { get; set; }
        public DbSet<Tratamiento> Tratamientos { get; set; }
        public DbSet<HistorialTratamiento> HistorialTratamientos { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Bitacora> Bitacora { get; set; }
        public DbSet<Procedimiento> Procedimientos { get; set; }
        public DbSet<PiezaDental> Odontograma { get; set; }
        public DbSet<PlanTratamiento> PlanesTratamiento { get; set; }
        public DbSet<InformacionMedica> InformacionMedica { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // === PACIENTE ===
            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.ToTable("pacientes");
                entity.HasKey(e => e.IdPaciente).HasName("PK_pacientes");
                entity.Property(e => e.IdPaciente).HasColumnName("id_paciente").ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Apellido).HasColumnName("apellido");
                entity.Property(e => e.Dni).HasColumnName("dni");
                entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
                entity.Property(e => e.Telefono).HasColumnName("telefono");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.Direccion).HasColumnName("direccion");
                entity.Property(e => e.FechaRegistro).HasColumnName("fecha_registro");
                entity.Property(e => e.Activo).HasColumnName("activo");
            });

            // === ODONTÓLOGO ===
            modelBuilder.Entity<Odontologo>(entity =>
            {
                entity.ToTable("odontologos");
                entity.HasKey(e => e.IdOdontologo).HasName("PK_odontologos");
                entity.Property(e => e.IdOdontologo).HasColumnName("id_odontologo").ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Apellido).HasColumnName("apellido");
                entity.Property(e => e.Matricula).HasColumnName("matricula");
                entity.Property(e => e.Telefono).HasColumnName("telefono");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.Especialidad).HasColumnName("especialidad");
            });

            // === TURNO ===
            modelBuilder.Entity<Turno>(entity =>
            {
                entity.ToTable("turnos");
                entity.HasKey(e => e.IdTurno).HasName("PK_turnos");
                entity.Property(e => e.IdTurno).HasColumnName("id_turno").ValueGeneratedOnAdd();
                entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
                entity.Property(e => e.IdOdontologo).HasColumnName("id_odontologo");
                entity.Property(e => e.FechaHora).HasColumnName("fecha_hora").HasColumnType("datetime");
                entity.Property(e => e.Estado).HasColumnName("estado");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");

                entity.HasOne(e => e.Paciente)
                      .WithMany(p => p.Turnos)
                      .HasForeignKey(e => e.IdPaciente);

                entity.HasOne(e => e.Odontologo)
                      .WithMany()
                      .HasForeignKey(e => e.IdOdontologo);
            });

            // === HISTORIAL CLÍNICO ===
            modelBuilder.Entity<HistorialClinico>(entity =>
            {
                entity.ToTable("historial_clinico");
                entity.HasKey(e => e.IdHistorial).HasName("PK_historial_clinico");
                entity.Property(e => e.IdHistorial).HasColumnName("id_historial").ValueGeneratedOnAdd();
                entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
                entity.Property(e => e.IdOdontologo).HasColumnName("id_odontologo");
                entity.Property(e => e.Fecha).HasColumnName("fecha");
                entity.Property(e => e.MotivoConsulta).HasColumnName("motivo_consulta");
                entity.Property(e => e.Diagnostico).HasColumnName("diagnostico");
                entity.Property(e => e.Observacion).HasColumnName("observacion");

                entity.HasOne(e => e.Paciente)
                      .WithMany(p => p.Historiales)
                      .HasForeignKey(e => e.IdPaciente);

                entity.HasOne(e => e.Odontologo)
                      .WithMany()
                      .HasForeignKey(e => e.IdOdontologo);
            });

            // === TRATAMIENTO ===
            modelBuilder.Entity<Tratamiento>(entity =>
            {
                entity.ToTable("tratamientos");
                entity.HasKey(e => e.IdTratamiento).HasName("PK_tratamientos");
                entity.Property(e => e.IdTratamiento).HasColumnName("id_tratamiento").ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Precio).HasColumnName("precio").HasColumnType("decimal(10,2)");
            });

            // === HISTORIAL TRATAMIENTO ===
            modelBuilder.Entity<HistorialTratamiento>(entity =>
            {
                entity.ToTable("historial_tratamientos");
                entity.HasKey(e => e.IdHistorialTratamiento).HasName("PK_historial_tratamientos");
                entity.Property(e => e.IdHistorialTratamiento).HasColumnName("id_historial_tratamiento").ValueGeneratedOnAdd();
                entity.Property(e => e.IdHistorial).HasColumnName("id_historial");
                entity.Property(e => e.IdTratamiento).HasColumnName("id_tratamiento");
                entity.Property(e => e.Cantidad).HasColumnName("cantidad");
                entity.Property(e => e.PrecioUnitario).HasColumnName("precio_unitario").HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.HistorialClinico)
                      .WithMany(h => h.HistorialTratamientos)
                      .HasForeignKey(e => e.IdHistorial);

                entity.HasOne(e => e.Tratamiento)
                      .WithMany()
                      .HasForeignKey(e => e.IdTratamiento);
            });

            // === PAGO ===
            modelBuilder.Entity<Pago>(entity =>
            {
                entity.ToTable("pagos");
                entity.HasKey(e => e.IdPago).HasName("PK_pagos");
                entity.Property(e => e.IdPago).HasColumnName("id_pago").ValueGeneratedOnAdd();
                entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
                entity.Property(e => e.FechaPago).HasColumnName("fecha_pago");
                entity.Property(e => e.Monto).HasColumnName("monto").HasColumnType("decimal(10,2)");
                entity.Property(e => e.MetodoPago).HasColumnName("metodo_pago");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");

                entity.HasOne(e => e.Paciente)
                      .WithMany(p => p.Pagos)
                      .HasForeignKey(e => e.IdPaciente);
            });

            // === USUARIO ===
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuarios");
                entity.HasKey(e => e.IdUsuario).HasName("PK_usuarios");
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario").ValueGeneratedOnAdd();
                entity.Property(e => e.Username).HasColumnName("username");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.Property(e => e.Rol).HasColumnName("rol");
                entity.Property(e => e.IdOdontologo).HasColumnName("id_odontologo");

                entity.HasOne(e => e.Odontologo)
                      .WithMany()
                      .HasForeignKey(e => e.IdOdontologo);
            });

            // === BITÁCORA ===
            modelBuilder.Entity<Bitacora>(entity =>
            {
                entity.ToTable("bitacora");
                entity.HasKey(e => e.IdBitacora).HasName("PK_bitacora");
                entity.Property(e => e.IdBitacora).HasColumnName("id_bitacora").ValueGeneratedOnAdd();
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
                entity.Property(e => e.Accion).HasColumnName("accion");
                entity.Property(e => e.Fecha).HasColumnName("fecha");
                entity.Property(e => e.Detalles).HasColumnName("detalles");

                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.Bitacoras)
                      .HasForeignKey(e => e.IdUsuario);
            });

            // === PIEZA DENTAL (ODONTOGRAMA) ===
            modelBuilder.Entity<PiezaDental>(entity =>
            {
                entity.ToTable("odontograma");
                entity.HasKey(e => e.IdPieza).HasName("PK_Odontograma");
                entity.Property(e => e.IdPieza).HasColumnName("id_pieza").ValueGeneratedOnAdd();
                entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
                entity.Property(e => e.NumeroPieza).HasColumnName("numero_pieza");
                entity.Property(e => e.Color).HasColumnName("color").HasMaxLength(20);
                entity.Property(e => e.Estado).HasColumnName("estado").HasMaxLength(50);
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");
                entity.Property(e => e.FechaActualizacion)
                      .HasColumnName("fecha_actualizacion")
                      .HasDefaultValueSql("GETDATE()");
            });

            // === PROCEDIMIENTO ===
            modelBuilder.Entity<Procedimiento>(entity =>
            {
                entity.ToTable("procedimientos");
                entity.HasKey(e => e.IdProcedimiento).HasName("PK_Procedimientos");
                entity.Property(e => e.IdProcedimiento).HasColumnName("id_procedimiento").ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(150);
                entity.Property(e => e.Categoria).HasColumnName("categoria").HasMaxLength(50);
                entity.Property(e => e.Costo).HasColumnName("costo").HasColumnType("decimal(10,2)");
                entity.Property(e => e.DuracionMinutos).HasColumnName("duracion_minutos");
                entity.Property(e => e.Activo).HasColumnName("activo");
            });

            // === PLAN TRATAMIENTO ===
            modelBuilder.Entity<PlanTratamiento>(entity =>
            {
                entity.ToTable("planes_tratamiento");
                entity.HasKey(e => e.IdPlanTratamiento).HasName("PK_PlanesTratamiento");
                entity.Property(e => e.IdPlanTratamiento).HasColumnName("id_plan_tratamiento").ValueGeneratedOnAdd();
                entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
                entity.Property(e => e.IdProcedimiento).HasColumnName("id_procedimiento");
                entity.Property(e => e.FechaPlan).HasColumnName("fecha_plan").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Estado).HasColumnName("estado").HasMaxLength(20);
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");

                entity.HasOne(e => e.Paciente)
                      .WithMany(p => p.PlanesTratamiento)
                      .HasForeignKey(e => e.IdPaciente);

                entity.HasOne(e => e.Procedimiento)
                      .WithMany()
                      .HasForeignKey(e => e.IdProcedimiento);
            });

            // === INFORMACIÓN MÉDICA ===
            modelBuilder.Entity<InformacionMedica>(entity =>
            {
                entity.ToTable("informacion_medica");
                entity.HasKey(e => e.IdInformacion).HasName("PK_informacion_medica");
                entity.Property(e => e.IdInformacion).HasColumnName("id_informacion").ValueGeneratedOnAdd();
                entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
                entity.Property(e => e.Alergias).HasColumnName("alergias");
                entity.Property(e => e.MedicacionesActuales).HasColumnName("medicaciones_actuales");
                entity.Property(e => e.AntecedentesMedicos).HasColumnName("antecedentes_medicos");
                entity.Property(e => e.AntecedentesOdontologicos).HasColumnName("antecedentes_odontologicos");
                entity.Property(e => e.Habitos).HasColumnName("habitos");
                entity.Property(e => e.ObservacionesMedicas).HasColumnName("observaciones_medicas");

                entity.HasOne(e => e.Paciente)
                      .WithMany(p => p.InformacionMedica)
                      .HasForeignKey(e => e.IdPaciente);
            });
        }
    }
}