namespace Confectionery.Models
{
    public static class ValidationMessages
    {
        public const string Required = "Поле обязательно для заполнения";
        public const string InvalidText = "Допустимы только буквы, цифры и стандартные символы";
        public const string InvalidLogin = "Логин может содержать только латинские буквы, цифры и символы ._-";
        public const string InvalidPassword = "Пароль должен содержать 6-16 символов без пробелов";
        public const string InvalidEmail = "Введите корректный email";
        public const string PasswordsDoNotMatch = "Пароли не совпадают";
        public const string InvalidName = "Имя может содержать только буквы и пробелы";
        public const string InvalidPrice = "Цена должна быть больше 0 и не более 100000 ₽";
        public const string RequiredPrice = "Введите цену товара";
        public const string InvalidQuantity = "Количество должно быть от 1 до 999";
        public const string InvalidImageType = "Разрешены только изображения (jpg, jpeg, png)";
        public const string ImageTooLarge = "Размер файла не должен превышать 5 МБ";
        public const string LoginExists = "Логин уже используется";
        public const string EmailExists = "Email уже зарегистрирован";
    }
}
