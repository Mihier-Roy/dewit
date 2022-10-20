using Dewit.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dewit.Infrastructure.Data.Config
{
	public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
	{
		public void Configure(EntityTypeBuilder<TaskItem> builder)
		{
			builder.HasKey(taskItem => taskItem.Id);

			builder.Property(taskItem => taskItem.TaskDescription)
				.IsRequired(true);

			builder.Property(taskItem => taskItem.Status)
				.IsRequired(true);

			builder.Property(taskItem => taskItem.Tags)
				.HasMaxLength(2048);
		}
	}
}