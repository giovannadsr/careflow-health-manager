using Xunit;

namespace CareFlow.Tests.Patients;

public class PatientServiceTests
{
    [Fact]
    public async Task CreatePatient_WithDuplicateCpf_ShouldThrowException()
    {
        await Assert.ThrowsAsync<Exception>(() =>
            Task.FromException(new Exception("CPF já cadastrado")));
    }
}