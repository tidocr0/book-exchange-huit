using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookExchange.Api.DTOs
{
    public class TimeSlotDto
    {
        public byte DayOfWeek { get; set; }
        public byte TimeSlot { get; set; }
    }

    public class UpdateAvailabilityDto
    {
        [Required]
        public List<TimeSlotDto> Slots { get; set; }
    }

    public class BlackoutDateDto
    {
        public int BlackoutId { get; set; }
        public DateTime BlackoutDate { get; set; }
        public byte? TimeSlot { get; set; }
    }

    public class CreateBlackoutDateDto
    {
        [Required]
        public DateTime BlackoutDate { get; set; }
        public byte? TimeSlot { get; set; }
    }

    public class ScheduleConfigDto
    {
        public List<TimeSlotDto> Availabilities { get; set; }
        public List<BlackoutDateDto> BlackoutDates { get; set; }
    }

    public class AvailableDateDto
    {
        public DateTime Date { get; set; }
        public List<byte> AvailableSlots { get; set; }
    }
}
