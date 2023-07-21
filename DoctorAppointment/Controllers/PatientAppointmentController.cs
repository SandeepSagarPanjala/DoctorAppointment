﻿using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

using Model = DoctorAppointment.Models.AppointmentModel;

namespace DoctorAppointment.Controllers;

[ApiController]
[Route("patient-appointment")]
public class PatientAppointmentController : ControllerBase
{
    private readonly NpgsqlConnection connection;

    public PatientAppointmentController(IConfiguration _configuration)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        this.connection = new NpgsqlConnection(connectionString);
    }

    ~PatientAppointmentController()
    {
        if(this.connection != null)
        {
            this.connection.Close();
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = @$"SELECT a.* FROM ""Patient"" p
                    JOIN ""Appointment"" a on a.""PatientId"" = p.""PatientId""
                    WHERE ""PatientId"" = {id}";
        var record = await connection.QuerySingleAsync<Model>(query);
        return Ok(record);
    }
}

