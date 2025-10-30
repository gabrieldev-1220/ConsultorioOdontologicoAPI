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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapeo explícito de entidades a tablas existentes
            modelBuilder.Entity<Paciente>().ToTable("pacientes");
            modelBuilder.Entity<Paciente>().Property(p => p.IdPaciente).HasColumnName("id_paciente");
            modelBuilder.Entity<Paciente>().Property(p => p.Nombre).HasColumnName("nombre");
            modelBuilder.Entity<Paciente>().Property(p => p.Apellido).HasColumnName("apellido");
            modelBuilder.Entity<Paciente>().Property(p => p.Dni).HasColumnName("dni");
            modelBuilder.Entity<Paciente>().Property(p => p.FechaNacimiento).HasColumnName("fecha_nacimiento");
            modelBuilder.Entity<Paciente>().Property(p => p.Telefono).HasColumnName("telefono");
            modelBuilder.Entity<Paciente>().Property(p => p.Email).HasColumnName("email");
            modelBuilder.Entity<Paciente>().Property(p => p.Direccion).HasColumnName("direccion");
            modelBuilder.Entity<Paciente>().Property(p => p.FechaRegistro).HasColumnName("fecha_registro");
            modelBuilder.Entity<Paciente>().Property(p => p.Activo).HasColumnName("activo");

            modelBuilder.Entity<Odontologo>().ToTable("odontologos");
            modelBuilder.Entity<Odontologo>().Property(o => o.IdOdontologo).HasColumnName("id_odontologo");
            modelBuilder.Entity<Odontologo>().Property(o => o.Nombre).HasColumnName("nombre");
            modelBuilder.Entity<Odontologo>().Property(o => o.Apellido).HasColumnName("apellido");
            modelBuilder.Entity<Odontologo>().Property(o => o.Matricula).HasColumnName("matricula");
            modelBuilder.Entity<Odontologo>().Property(o => o.Telefono).HasColumnName("telefono");
            modelBuilder.Entity<Odontologo>().Property(o => o.Email).HasColumnName("email");
            modelBuilder.Entity<Odontologo>().Property(o => o.Especialidad).HasColumnName("especialidad");

            modelBuilder.Entity<Turno>().ToTable("turnos");
            modelBuilder.Entity<Turno>().Property(t => t.IdTurno).HasColumnName("id_turno");
            modelBuilder.Entity<Turno>().Property(t => t.IdPaciente).HasColumnName("id_paciente");
            modelBuilder.Entity<Turno>().Property(t => t.IdOdontologo).HasColumnName("id_odontologo");
            modelBuilder.Entity<Turno>().Property(t => t.FechaHora).HasColumnName("fecha_hora").HasColumnType("datetime");
            modelBuilder.Entity<Turno>().Property(t => t.Estado).HasColumnName("estado");
            modelBuilder.Entity<Turno>().Property(t => t.Observaciones).HasColumnName("observaciones");

            modelBuilder.Entity<HistorialClinico>().ToTable("historial_clinico");
            modelBuilder.Entity<HistorialClinico>().Property(h => h.IdHistorial).HasColumnName("id_historial");
            modelBuilder.Entity<HistorialClinico>().Property(h => h.IdPaciente).HasColumnName("id_paciente");
            modelBuilder.Entity<HistorialClinico>().Property(h => h.IdOdontologo).HasColumnName("id_odontologo");
            modelBuilder.Entity<HistorialClinico>().Property(h => h.Fecha).HasColumnName("fecha");
            modelBuilder.Entity<HistorialClinico>().Property(h => h.MotivoConsulta).HasColumnName("motivo_consulta");
            modelBuilder.Entity<HistorialClinico>().Property(h => h.Diagnostico).HasColumnName("diagnostico");
            modelBuilder.Entity<HistorialClinico>().Property(h => h.Observacion).HasColumnName("observacion");

            modelBuilder.Entity<Tratamiento>().ToTable("tratamientos");
            modelBuilder.Entity<Tratamiento>().Property(t => t.IdTratamiento).HasColumnName("id_tratamiento");
            modelBuilder.Entity<Tratamiento>().Property(t => t.Nombre).HasColumnName("nombre");
            modelBuilder.Entity<Tratamiento>().Property(t => t.Descripcion).HasColumnName("descripcion");
            modelBuilder.Entity<Tratamiento>().Property(t => t.Precio).HasColumnName("precio").HasColumnType("decimal(10,2)");

            modelBuilder.Entity<HistorialTratamiento>().ToTable("historial_tratamientos");
            modelBuilder.Entity<HistorialTratamiento>().Property(h => h.IdHistorialTratamiento).HasColumnName("id_historial_tratamiento");
            modelBuilder.Entity<HistorialTratamiento>().Property(h => h.IdHistorial).HasColumnName("id_historial");
            modelBuilder.Entity<HistorialTratamiento>().Property(h => h.IdTratamiento).HasColumnName("id_tratamiento");
            modelBuilder.Entity<HistorialTratamiento>().Property(h => h.Cantidad).HasColumnName("cantidad");
            modelBuilder.Entity<HistorialTratamiento>().Property(h => h.PrecioUnitario).HasColumnName("precio_unitario").HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Pago>().ToTable("pagos");
            modelBuilder.Entity<Pago>().Property(p => p.IdPago).HasColumnName("id_pago");
            modelBuilder.Entity<Pago>().Property(p => p.IdPaciente).HasColumnName("id_paciente");
            modelBuilder.Entity<Pago>().Property(p => p.FechaPago).HasColumnName("fecha_pago");
            modelBuilder.Entity<Pago>().Property(p => p.Monto).HasColumnName("monto").HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Pago>().Property(p => p.MetodoPago).HasColumnName("metodo_pago");
            modelBuilder.Entity<Pago>().Property(p => p.Observaciones).HasColumnName("observaciones");

            modelBuilder.Entity<Usuario>().ToTable("usuarios");
            modelBuilder.Entity<Usuario>().Property(u => u.IdUsuario).HasColumnName("id_usuario");
            modelBuilder.Entity<Usuario>().Property(u => u.Username).HasColumnName("username");
            modelBuilder.Entity<Usuario>().Property(u => u.PasswordHash).HasColumnName("password_hash");
            modelBuilder.Entity<Usuario>().Property(u => u.Rol).HasColumnName("rol");
            modelBuilder.Entity<Usuario>().Property(u => u.IdOdontologo).HasColumnName("id_odontologo");

            modelBuilder.Entity<Bitacora>().ToTable("bitacora");
            modelBuilder.Entity<Bitacora>().Property(b => b.IdBitacora).HasColumnName("id_bitacora");
            modelBuilder.Entity<Bitacora>().Property(b => b.IdUsuario).HasColumnName("id_usuario");
            modelBuilder.Entity<Bitacora>().Property(b => b.Accion).HasColumnName("accion");
            modelBuilder.Entity<Bitacora>().Property(b => b.Fecha).HasColumnName("fecha");
            modelBuilder.Entity<Bitacora>().Property(b => b.Detalles).HasColumnName("detalles");

            // Configuración de relaciones (claves foráneas)
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Paciente)
                .WithMany(p => p.Turnos)
                .HasForeignKey(t => t.IdPaciente);

            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Odontologo)
                .WithMany()
                .HasForeignKey(t => t.IdOdontologo);

            modelBuilder.Entity<HistorialClinico>()
                .HasOne(h => h.Paciente)
                .WithMany(p => p.Historiales)
                .HasForeignKey(h => h.IdPaciente);

            modelBuilder.Entity<HistorialClinico>()
                .HasOne(h => h.Odontologo)
                .WithMany()
                .HasForeignKey(h => h.IdOdontologo);

            modelBuilder.Entity<HistorialTratamiento>()
                .HasOne(h => h.HistorialClinico)
                .WithMany(h => h.HistorialTratamientos)
                .HasForeignKey(h => h.IdHistorial);

            modelBuilder.Entity<HistorialTratamiento>()
                .HasOne(h => h.Tratamiento)
                .WithMany()
                .HasForeignKey(h => h.IdTratamiento);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Paciente)
                .WithMany(p => p.Pagos)
                .HasForeignKey(p => p.IdPaciente);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Odontologo)
                .WithMany()
                .HasForeignKey(u => u.IdOdontologo);

            modelBuilder.Entity<Bitacora>()
                .HasOne(b => b.Usuario)
                .WithMany(u => u.Bitacoras)
                .HasForeignKey(b => b.IdUsuario);

            // Mapear la entidad PiezaDental a la tabla odontograma
            modelBuilder.Entity<PiezaDental>(entity =>
            {
                entity.ToTable("odontograma");

                entity.HasKey(e => e.IdPieza).HasName("PK_Odontograma");

                entity.Property(e => e.IdPieza)
                    .HasColumnName("id_pieza")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
                entity.Property(e => e.NumeroPieza).HasColumnName("numero_pieza");
                entity.Property(e => e.Color).HasColumnName("color").HasMaxLength(20);
                entity.Property(e => e.Estado).HasColumnName("estado").HasMaxLength(50);
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");
                entity.Property(e => e.FechaActualizacion)
                      .HasColumnName("fecha_actualizacion")
                      .HasDefaultValueSql("GETDATE()");
            });

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

            // Mapear la entidad PlanTratamiento a la tabla planes_tratamiento
            modelBuilder.Entity<PlanTratamiento>(entity =>
            {
                entity.ToTable("planes_tratamiento");

                entity.HasKey(e => e.IdPlanTratamiento).HasName("PK_PlanesTratamiento");

                entity.Property(e => e.IdPlanTratamiento)
                    .HasColumnName("id_plan_tratamiento")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
                entity.Property(e => e.IdProcedimiento).HasColumnName("id_procedimiento");
                entity.Property(e => e.FechaPlan).HasColumnName("fecha_plan").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Estado).HasColumnName("estado").HasMaxLength(20);
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");

                entity.HasOne(pt => pt.Paciente)
                    .WithMany(p => p.PlanesTratamiento)
                    .HasForeignKey(pt => pt.IdPaciente);

                entity.HasOne(pt => pt.Procedimiento)
                    .WithMany()
                    .HasForeignKey(pt => pt.IdProcedimiento);
            });
        }
    }
}