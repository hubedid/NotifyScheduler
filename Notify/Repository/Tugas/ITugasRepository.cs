namespace Notify.Repository.Tugas;

public interface ITugasRepository
{
    IEnumerable<TugasModel> GetAllTugas();
    TugasModel GetTugasById(string tugasId);
    int SetJobKeyByTugasId(string tugasId, string jobKey);
}
