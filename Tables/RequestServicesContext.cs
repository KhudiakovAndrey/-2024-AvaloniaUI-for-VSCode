using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RequestServices_Ivanov;

public partial class RequestServicesContext : DbContext
{
    public RequestServicesContext()
    {
    }

    public RequestServicesContext(DbContextOptions<RequestServicesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Equipment> Equipments { get; set; }

    public virtual DbSet<Executor> Executors { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<OrderSparePart> OrderSpareParts { get; set; }

    public virtual DbSet<ReleaseRequest> ReleaseRequests { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SparePart> SpareParts { get; set; }

    public virtual DbSet<TypeProblem> TypeProblems { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseLazyLoadingProxies().UseSqlServer("Server=DESKTOP-DJ98FFV\\PAVILYON;Database=RequestServices;Trusted_Connection=true;TrustServerCertificate=True");
    //"Server=DESKTOP-BJ79SNE\\SQLEXPRESSNET_59;Database=RequestServices;User ID=test;Password=testadmin;TrustServerCertificate=True"
    //"Server=DESKTOP-DJ98FFV\\PAVILYON;Database=RequestServices;Trusted_Connection=true;TrustServerCertificate=True"
    //"Server=43-01;Database=RequestServices;Trusted_Connection=true;TrustServerCertificate=True"

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Idclient);

            entity.ToTable("CLIENTS");

            entity.Property(e => e.Idclient).HasColumnName("IDClient");
            entity.Property(e => e.Iduser).HasColumnName("IDUser");
            entity.Property(e => e.Phone).HasMaxLength(15);

            entity.HasOne(d => d.IduserNavigation).WithMany(p => p.Clients)
                .HasForeignKey(d => d.Iduser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CLIENTS_USERS");
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.Idequipment);

            entity.ToTable("EQUIPMENTS");

            entity.Property(e => e.Idequipment).HasColumnName("IDEquipment");
            entity.Property(e => e.IdtypeProblem).HasColumnName("IDTypeProblem");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.TypeEquipment).HasMaxLength(100);

            entity.HasOne(d => d.IdtypeProblemNavigation).WithMany(p => p.Equipment)
                .HasForeignKey(d => d.IdtypeProblem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EQUIPMENTS_TYPE_PROBLEM");
        });

        modelBuilder.Entity<Executor>(entity =>
        {
            entity.HasKey(e => e.Idexecutor);

            entity.ToTable("EXECUTORS");

            entity.Property(e => e.Idexecutor).HasColumnName("IDExecutor");
            entity.Property(e => e.IdtypeProblem).HasColumnName("IDTypeProblem");
            entity.Property(e => e.Iduser).HasColumnName("IDUser");

            entity.HasOne(d => d.IdtypeProblemNavigation).WithMany(p => p.Executors)
                .HasForeignKey(d => d.IdtypeProblem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EXECUTORS_TYPE_PROBLEM");

            entity.HasOne(d => d.IduserNavigation).WithMany(p => p.Executors)
                .HasForeignKey(d => d.Iduser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EXECUTORS_USERS");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Idrequest);

            entity.ToTable("FEEDBACKS");

            entity.Property(e => e.Idrequest)
                .ValueGeneratedOnAdd()
                .HasColumnName("IDRequest");

            entity.HasOne(d => d.IdrequestNavigation).WithOne(p => p.Feedback)
                .HasForeignKey<Feedback>(d => d.Idrequest)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FEEDBACKS_REQUESTS");
        });

        modelBuilder.Entity<OrderSparePart>(entity =>
        {
            entity.HasKey(e => e.IdorderSparePart);

            entity.ToTable("ORDER_SPARE_PART");

            entity.Property(e => e.IdorderSparePart).HasColumnName("IDOrderSparePart");
            entity.Property(e => e.Idequipment).HasColumnName("IDEquipment");
            entity.Property(e => e.IdsparePart).HasColumnName("IDSparePart");

            entity.HasOne(d => d.IdequipmentNavigation).WithMany(p => p.OrderSpareParts)
                .HasForeignKey(d => d.Idequipment)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ORDER_SPARE_PART_EQUIPMENTS");

            entity.HasOne(d => d.IdsparePartNavigation).WithMany(p => p.OrderSpareParts)
                .HasForeignKey(d => d.IdsparePart)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ORDER_SPARE_PART_SPARE_PARTS");
        });

        modelBuilder.Entity<ReleaseRequest>(entity =>
        {
            entity.HasKey(e => e.IdreleaseRequests);

            entity.ToTable("RELEASE_REQUESTS");

            entity.Property(e => e.IdreleaseRequests).HasColumnName("IDReleaseRequests");
            entity.Property(e => e.Idexecutor).HasColumnName("IDExecutor");
            entity.Property(e => e.Idrequest).HasColumnName("IDRequest");

            entity.HasOne(d => d.IdexecutorNavigation).WithMany(p => p.ReleaseRequests)
                .HasForeignKey(d => d.Idexecutor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RELEASE_REQUESTS_EXECUTORS");

            entity.HasOne(d => d.IdrequestNavigation).WithMany(p => p.ReleaseRequests)
                .HasForeignKey(d => d.Idrequest)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RELEASE_REQUESTS_REQUESTS");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Idrequest);

            entity.ToTable("REQUESTS");

            entity.Property(e => e.Idrequest).HasColumnName("IDRequest");
            entity.Property(e => e.DateAdd).HasColumnType("datetime");
            entity.Property(e => e.Idclient).HasColumnName("IDClient");
            entity.Property(e => e.Idequipment).HasColumnName("IDEquipment");
            entity.Property(e => e.Idexecutor).HasColumnName("IDExecutor");
            entity.Property(e => e.Priority).HasMaxLength(20);
            entity.Property(e => e.Status).HasMaxLength(30);

            entity.HasOne(d => d.IdclientNavigation).WithMany(p => p.Requests)
                .HasForeignKey(d => d.Idclient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_REQUESTS_CLIENTS");

            entity.HasOne(d => d.IdequipmentNavigation).WithMany(p => p.Requests)
                .HasForeignKey(d => d.Idequipment)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_REQUESTS_EQUIPMENTS");

            entity.HasOne(d => d.IdexecutorNavigation).WithMany(p => p.Requests)
                .HasForeignKey(d => d.Idexecutor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_REQUESTS_EXECUTORS");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Idrole);

            entity.ToTable("ROLE");

            entity.Property(e => e.Idrole).HasColumnName("IDRole");
            entity.Property(e => e.Title).HasMaxLength(20);
        });

        modelBuilder.Entity<SparePart>(entity =>
        {
            entity.HasKey(e => e.IdsparePart);

            entity.ToTable("SPARE_PARTS");

            entity.Property(e => e.IdsparePart).HasColumnName("IDSparePart");
            entity.Property(e => e.Cost).HasColumnType("money");
            entity.Property(e => e.DeliveryTime).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(40);
        });

        modelBuilder.Entity<TypeProblem>(entity =>
        {
            entity.HasKey(e => e.IdtypeProblem);

            entity.ToTable("TYPE_PROBLEM");

            entity.Property(e => e.IdtypeProblem).HasColumnName("IDTypeProblem");
            entity.Property(e => e.Cost).HasColumnType("money");
            entity.Property(e => e.Title).HasMaxLength(40);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Iduser);

            entity.ToTable("USERS");

            entity.Property(e => e.Iduser).HasColumnName("IDUser");
            entity.Property(e => e.Fio)
                .HasMaxLength(200)
                .HasColumnName("FIO");
            entity.Property(e => e.Idrole).HasColumnName("IDRole");
            entity.Property(e => e.Login).HasMaxLength(20);
            entity.Property(e => e.Password).HasMaxLength(20);

            entity.HasOne(d => d.IdroleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Idrole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USERS_ROLE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
