namespace ElasticLogBuilder
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Тип изменения  лога
    /// </summary>
    public enum ActionType
    {
        [Display(Name = "Добавили")]
        Create = 0,

        [Display(Name = "Изменили")]
        Edit = 10,

        [Display(Name = "Удалили")]
        Delete = 20,

        [Display(Name = "Добавили в лог")]
        Actualization = 30
    }
}
