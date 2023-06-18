using Dewit.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dewit.Infrastructure.Data.Config
{
	public class JournalItemConfiguration : IEntityTypeConfiguration<JournalItem>
	{
		public void Configure(EntityTypeBuilder<JournalItem> builder)
		{
			builder.HasKey(journalItem => journalItem.Id);

			builder.Property(journalItem => journalItem.CalendarDate)
				.IsRequired(true);

			builder.Property(journalItem => journalItem.Mood)
				.IsRequired(true);

			builder.Property(journalItem => journalItem.Note);
		}
	}
}