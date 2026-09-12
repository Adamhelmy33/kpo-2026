package studying.withsolid.exception;

/**
 * Коды ошибок приложения, используемые в {@link ApplicationException}.
 */
public enum ApplicationErrorCode {

    /**
     * Ошибка валидации входных данных.
     */
    VALIDATION_ERROR,

    /**
     * Ошибка записи файла на диск.
     */
    FILE_WRITE_ERROR
}
