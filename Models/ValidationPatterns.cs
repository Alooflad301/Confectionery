namespace Confectionery.Models
{
    public static class ValidationPatterns
    {
        public const int ShortTextMaxLength = 50;
        public const int MediumTextMaxLength = 100;
        public const int LongTextMaxLength = 250;

        // Безопасный текст
        public const string SafeTextPattern = @"^[a-zA-Zа-яА-ЯёЁ0-9\s\-\.]+$";

        // Логин
        public const string LoginPattern = @"^[a-zA-Z0-9._-]+$";

        // Пароль 6-16 символов без пробелов
        public const string PasswordPattern = @"^(?=.{6,16}$)[^\s]+$";

        // Имя (только буквы и пробелы)
        public const string NamePattern = @"^[а-яА-ЯёЁa-zA-Z\s]+$";

        // Цена
        public const decimal PriceMaxValue = 100000m;
        public const int QuantityMaxValue = 999;
    }
}
