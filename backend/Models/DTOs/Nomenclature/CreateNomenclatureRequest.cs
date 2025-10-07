using System.ComponentModel;
using System.Text.Json.Serialization;

namespace backend.Models.Nomenclature
{
    public class CreateNomenclatureRequest
    {
        [Description("Уникальный идентификатор номенклатуры")]
        public int ID { get; set; }

        [Description("Идентификатор категории")]
        public string IDCat { get; set; } = string.Empty;

        [Description("Идентификатор типа продукции")]
        public int IDType { get; set; }

        [Description("Новый идентификатор типа продукции")]
        public string IDTypeNew { get; set; } = string.Empty;

        [Description("Тип производства")]
        public string ProductionType { get; set; } = string.Empty;

        [Description("Тип функционального назначения")]
        public string IDFunctionType { get; set; } = string.Empty;

        [Description("Наименование номенклатуры")]
        public string Name { get; set; } = string.Empty;

        [Description("ГОСТ стандарт")]
        public string Gost { get; set; } = string.Empty;

        [Description("Форма поставки по длине")]
        public string FormOfLength { get; set; } = string.Empty;

        [Description("Производитель")]
        public string Manufacturer { get; set; } = string.Empty;

        [Description("Марка стали")]
        public string SteelGrade { get; set; } = string.Empty;

        [Description("Диаметр трубы (мм)")]
        public decimal Diameter { get; set; }

        [Description("Второй размер профиля (мм) - для профильных труб")]
        public decimal? ProfileSize2 { get; set; }

        [Description("Толщина стенки трубы (мм)")]
        public decimal PipeWallThickness { get; set; }

        [Description("Статус номенклатуры (активна/неактивна)")]
        public string Status { get; set; } = string.Empty;

        [Description("Коэффициент пересчета метров в тонны")]
        public decimal Koef { get; set; }
    }
}