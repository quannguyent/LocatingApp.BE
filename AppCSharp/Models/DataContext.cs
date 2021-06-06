using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LocatingApp.Models
{
    public partial class DataContext : DbContext
    {
        public virtual DbSet<AggregatedCounterDAO> AggregatedCounter { get; set; }
        public virtual DbSet<AppUserDAO> AppUser { get; set; }
        public virtual DbSet<AppUserAppUserMappingDAO> AppUserAppUserMapping { get; set; }
        public virtual DbSet<CheckingStatusDAO> CheckingStatus { get; set; }
        public virtual DbSet<CounterDAO> Counter { get; set; }
        public virtual DbSet<FileDAO> File { get; set; }
        public virtual DbSet<HashDAO> Hash { get; set; }
        public virtual DbSet<JobDAO> Job { get; set; }
        public virtual DbSet<JobParameterDAO> JobParameter { get; set; }
        public virtual DbSet<JobQueueDAO> JobQueue { get; set; }
        public virtual DbSet<ListDAO> List { get; set; }
        public virtual DbSet<LocationLogDAO> LocationLog { get; set; }
        public virtual DbSet<PageDAO> Page { get; set; }
        public virtual DbSet<PlaceDAO> Place { get; set; }
        public virtual DbSet<PlaceCheckingDAO> PlaceChecking { get; set; }
        public virtual DbSet<PlaceGroupDAO> PlaceGroup { get; set; }
        public virtual DbSet<RoleDAO> Role { get; set; }
        public virtual DbSet<SchemaDAO> Schema { get; set; }
        public virtual DbSet<ServerDAO> Server { get; set; }
        public virtual DbSet<SetDAO> Set { get; set; }
        public virtual DbSet<SexDAO> Sex { get; set; }
        public virtual DbSet<StateDAO> State { get; set; }
        public virtual DbSet<StatusDAO> Status { get; set; }
        public virtual DbSet<TrackingDAO> Tracking { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("data source=112.213.88.49,1500;initial catalog=LocatingApp;persist security info=True;user id=sa;password=123@123a;multipleactiveresultsets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AggregatedCounterDAO>(entity =>
            {
                entity.HasKey(e => e.Key)
                    .HasName("PK_HangFire_CounterAggregated");

                entity.ToTable("AggregatedCounter", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_AggregatedCounter_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<AppUserDAO>(entity =>
            {
                entity.Property(e => e.Avatar).HasMaxLength(4000);

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.OtpCode).HasMaxLength(4000);

                entity.Property(e => e.OtpExpired).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Phone).HasMaxLength(4000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<AppUserAppUserMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.AppUserId, e.FriendId })
                    .HasName("PK_UserRelationship");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.AppUserAppUserMappingAppUsers)
                    .HasForeignKey(d => d.AppUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUserAppUserMapping_AppUser");

                entity.HasOne(d => d.Friend)
                    .WithMany(p => p.AppUserAppUserMappingFriends)
                    .HasForeignKey(d => d.FriendId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUserAppUserMapping_AppUser1");
            });

            modelBuilder.Entity<CheckingStatusDAO>(entity =>
            {
                entity.ToTable("CheckingStatus", "ENUM");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<CounterDAO>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Counter", "HangFire");

                entity.HasIndex(e => e.Key)
                    .HasName("CX_HangFire_Counter")
                    .IsClustered();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<FileDAO>(entity =>
            {
                entity.Property(e => e.Content).HasMaxLength(4000);

                entity.Property(e => e.GridId).HasMaxLength(4000);

                entity.Property(e => e.Key).HasMaxLength(4000);

                entity.Property(e => e.MimeType).HasMaxLength(4000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.Path).HasMaxLength(4000);
            });

            modelBuilder.Entity<HashDAO>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Field })
                    .HasName("PK_HangFire_Hash");

                entity.ToTable("Hash", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_Hash_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Field).HasMaxLength(100);
            });

            modelBuilder.Entity<JobDAO>(entity =>
            {
                entity.ToTable("Job", "HangFire");

                entity.HasIndex(e => e.StateName)
                    .HasName("IX_HangFire_Job_StateName")
                    .HasFilter("([StateName] IS NOT NULL)");

                entity.HasIndex(e => new { e.StateName, e.ExpireAt })
                    .HasName("IX_HangFire_Job_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Arguments).IsRequired();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.InvocationData).IsRequired();

                entity.Property(e => e.StateName).HasMaxLength(20);
            });

            modelBuilder.Entity<JobParameterDAO>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Name })
                    .HasName("PK_HangFire_JobParameter");

                entity.ToTable("JobParameter", "HangFire");

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobParameters)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_JobParameter_Job");
            });

            modelBuilder.Entity<JobQueueDAO>(entity =>
            {
                entity.HasKey(e => new { e.Queue, e.Id })
                    .HasName("PK_HangFire_JobQueue");

                entity.ToTable("JobQueue", "HangFire");

                entity.Property(e => e.Queue).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FetchedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ListDAO>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Id })
                    .HasName("PK_HangFire_List");

                entity.ToTable("List", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_List_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<LocationLogDAO>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Latitude).HasColumnType("decimal(8, 2)");

                entity.Property(e => e.Longtitude).HasColumnType("decimal(8, 2)");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.LocationLogs)
                    .HasForeignKey(d => d.AppUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Location_AppUser");
            });

            modelBuilder.Entity<PageDAO>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Path).HasMaxLength(500);
            });

            modelBuilder.Entity<PlaceDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Latitude).HasColumnType("decimal(8, 2)");

                entity.Property(e => e.Longtitude).HasColumnType("decimal(8, 2)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.PlaceGroup)
                    .WithMany(p => p.Places)
                    .HasForeignKey(d => d.PlaceGroupId)
                    .HasConstraintName("FK_PlaceTracking_PlaceGroup1");
            });

            modelBuilder.Entity<PlaceCheckingDAO>(entity =>
            {
                entity.Property(e => e.CheckInAt).HasColumnType("datetime");

                entity.Property(e => e.CheckOutAt).HasColumnType("datetime");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.PlaceCheckings)
                    .HasForeignKey(d => d.AppUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlaceChecking_AppUser");

                entity.HasOne(d => d.PlaceCheckingStatus)
                    .WithMany(p => p.PlaceCheckings)
                    .HasForeignKey(d => d.PlaceCheckingStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlaceChecking_CheckingStatus");

                entity.HasOne(d => d.Place)
                    .WithMany(p => p.PlaceCheckings)
                    .HasForeignKey(d => d.PlaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlaceChecking_Place");
            });

            modelBuilder.Entity<PlaceGroupDAO>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_PlaceGroup_PlaceGroup");
            });

            modelBuilder.Entity<RoleDAO>(entity =>
            {
                entity.ToTable("Role", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<SchemaDAO>(entity =>
            {
                entity.HasKey(e => e.Version)
                    .HasName("PK_HangFire_Schema");

                entity.ToTable("Schema", "HangFire");

                entity.Property(e => e.Version).ValueGeneratedNever();
            });

            modelBuilder.Entity<ServerDAO>(entity =>
            {
                entity.ToTable("Server", "HangFire");

                entity.HasIndex(e => e.LastHeartbeat)
                    .HasName("IX_HangFire_Server_LastHeartbeat");

                entity.Property(e => e.Id).HasMaxLength(100);

                entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
            });

            modelBuilder.Entity<SetDAO>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Value })
                    .HasName("PK_HangFire_Set");

                entity.ToTable("Set", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_Set_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => new { e.Key, e.Score })
                    .HasName("IX_HangFire_Set_Score");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Value).HasMaxLength(256);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<SexDAO>(entity =>
            {
                entity.ToTable("Sex", "ENUM");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<StateDAO>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Id })
                    .HasName("PK_HangFire_State");

                entity.ToTable("State", "HangFire");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Reason).HasMaxLength(100);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.States)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_State_Job");
            });

            modelBuilder.Entity<StatusDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<TrackingDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.PlaceChecking)
                    .WithMany(p => p.Trackings)
                    .HasForeignKey(d => d.PlaceCheckingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tracking_PlaceChecking");

                entity.HasOne(d => d.Place)
                    .WithMany(p => p.Trackings)
                    .HasForeignKey(d => d.PlaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tracking_Place");

                entity.HasOne(d => d.Target)
                    .WithMany(p => p.TrackingTargets)
                    .HasForeignKey(d => d.TargetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tracking_AppUser1");

                entity.HasOne(d => d.Tracker)
                    .WithMany(p => p.TrackingTrackers)
                    .HasForeignKey(d => d.TrackerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tracking_AppUser");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
