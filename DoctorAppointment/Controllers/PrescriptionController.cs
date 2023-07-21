using Dapper;
using DoctorAppointment.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

using Model = DoctorAppointment.Models.PrescriptionModel;

namespace DoctorAppointment.Controllers;

[ApiController]
[Route("[controller]")]
public class PrescriptionController : ControllerBase
{
    private readonly NpgsqlConnection connection;

    public PrescriptionController(IConfiguration _configuration)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        this.connection = new NpgsqlConnection(connectionString);
    }

    ~PrescriptionController()
    {
        if(this.connection != null)
        {
            this.connection.Close();
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromBody] AppointmentFilterModel model)
    {
        var where = "WHERE 1 = 1";

        if(string.IsNullOrEmpty(model.PatientName))
        {
            where += $@" AND p.""PatientName"" like '%{model.PatientName}%' ";
        }

        if (string.IsNullOrEmpty(model.DoctorName))
        {
            where += $@" AND d.""DoctorName"" like '%{model.DoctorName}%' ";
        }

        if (model.AppointmentDate != null)
        {
            where += $@" AND a.""AppointmentDate"" = '{model.AppointmentDate?.Date}' ";
        }

        var query = @$"SELECT a.*, p.""Name"" AS ""PatientName"", d.""Name"" AS ""DoctorName""  FROM ""Appointment"" a
                        JOIN ""Patient"" p on p.""PatientId"" = a.""PatientId""
                        JOIN ""Doctor"" d on d.""DoctorId"" = a.""DoctorId"" {where} ";
        var records = await connection.QueryAsync<AppointmentFetchModel>(query);
        return Ok(records);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        return await this.GetByIdHelper(id);
    }

    private async Task<IActionResult> GetByIdHelper(int id)
    {
        var query = @$"SELECT * FROM ""Prescription"" WHERE ""PrescriptionId"" = {id}";
        var record = await connection.QuerySingleAsync<Model>(query);
        return Ok(record);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Model model)
    {
        var query = $@"INSERT INTO ""Prescription"" (""AppointmentId"", ""MedicationName"", ""Dosage"", ""Instructions"")
                        VALUES ({model.AppointmentId}, {model.MedicationName}, '{model.Dosage}', '{model.Instructions}')
                        RETURNING ""PrescriptionId""";
        var newId = await this.connection.ExecuteScalarAsync(query);
        return await this.GetByIdHelper((int)newId);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Model model)
    {
        var query = $@"UPDATE ""Prescription"" SET ""MedicationName"" = '{model.MedicationName}', ""Dosage"" = '{model.Dosage}',
                        ""Instructions"" = '{model.Instructions}' WHERE ""PrescriptionId"" = {id} ";
        var updatedCount = await this.connection.ExecuteAsync(query);
        return Ok(updatedCount);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var query = $@"DELETE FROM ""Prescription"" WHERE ""PrescriptionId"" = {id} ";
        var deletedCount = await this.connection.ExecuteAsync(query);
        return Ok(deletedCount);
    }
}

