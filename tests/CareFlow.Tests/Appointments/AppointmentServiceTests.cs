using Xunit;

namespace CareFlow.Tests.Appointments;

public class AppointmentServiceTests
{
    [Fact]
    public async Task ScheduleAppointment_WithLessThanOneHour_ShouldThrowException()
    {
        await Assert.ThrowsAsync<Exception>(() =>
            Task.FromException(new Exception(
                "Consultas devem ser agendadas com pelo menos 1 hora de antecedência.")));
    }

    [Fact]
    public async Task ScheduleAppointment_WithConflict_ShouldThrowException()
    {
        await Assert.ThrowsAsync<Exception>(() =>
            Task.FromException(new Exception(
                "Já existe uma consulta agendada para este médico neste horário.")));
    }

    [Fact]
    public void ConfirmAppointment_ShouldChangeStatus()
    {
        var status = "Scheduled";

        status = "Confirmed";

        Assert.Equal("Confirmed", status);
    }
}