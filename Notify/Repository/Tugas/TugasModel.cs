namespace Notify.Repository.Tugas;

public class TugasModel
{
    public string TugasId { get; set; }
    public string Judul { get; set; }
    public string Deskripsi { get; set; }
    public DateTime Deadline { get; set; }
    public int Status { get; set; }
    public string PenggunaId { get; set; }
    public string KategoriId { get; set; }
    public string LabelId { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
}
