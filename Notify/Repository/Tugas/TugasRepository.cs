using SqlKata;
using SqlKata.Execution;

namespace Notify.Repository.Tugas;

public class TugasRepository : ITugasRepository
{
    private readonly QueryFactory _db;

    public TugasRepository(QueryFactory db)
    {
        _db = db;
    }
    public IEnumerable<TugasModel> GetAllTugas()
    {
        ICollection<TugasModel> tugasRes = [];
        IEnumerable<dynamic>? tugas = _db.Get(new Query("tugas").Join("pengguna","pengguna.ID_Pengguna", "tugas.ID_Pengguna"));

        foreach (dynamic tuga in tugas)
        {
            tugasRes.Add(new TugasModel()
            {
                TugasId = tuga.ID_Tugas,
                Judul = tuga.Judul,
                Deskripsi = tuga.Deskripsi,
                Deadline = tuga.Deadline,
                Status = tuga.Status,
                PenggunaId = tuga.ID_Pengguna,
                KategoriId = tuga.ID_Kategori,
                LabelId = tuga.ID_label,
                Email = tuga.Email,
                Username = tuga.Username,
                JobKey = tuga.job_key
            });
        }

        return tugasRes;
    }

    public TugasModel GetTugasById(string tugasId)
    {
        dynamic? tuga = _db.Query("tugas")
            .Join("pengguna", "pengguna.ID_Pengguna", "tugas.ID_Pengguna")
            .Where("ID_Tugas", tugasId)
            .FirstOrDefault();

        if (tuga == null)
        {
            return new TugasModel();
        }

        return new TugasModel()
        {
            TugasId = tuga.ID_Tugas,
            Judul = tuga.Judul,
            Deskripsi = tuga.Deskripsi,
            Deadline = tuga.Deadline,
            Status = tuga.Status,
            PenggunaId = tuga.ID_Pengguna,
            KategoriId = tuga.ID_Kategori,
            LabelId = tuga.ID_label,
            Email = tuga.Email,
            Username = tuga.Username,
            JobKey = tuga.job_key
        };
    }

    public int SetJobKeyByTugasId(string tugasId, string jobKey)
    {
        int affected = _db.Query("tugas")
            .Where("ID_Tugas", tugasId)
            .Update(new
            {
                job_key = jobKey
            });

        return affected;
    }
}
