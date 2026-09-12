package studying.withsolid.exception;

/**
 * Исключение уровня приложения, дополнительно несущее код ошибки {@link ApplicationErrorCode}.
 */
public class ApplicationException extends RuntimeException {

    private final ApplicationErrorCode errorCode;

    /**
     * Создает исключение с указанным кодом ошибки и сообщением.
     *
     * @param errorCode код ошибки
     * @param message   сообщение об ошибке
     */
    public ApplicationException(ApplicationErrorCode errorCode, String message) {
        super(message);
        this.errorCode = errorCode;
    }

    /**
     * Создает исключение с указанным кодом ошибки, сообщением и первопричиной.
     *
     * @param errorCode код ошибки
     * @param message   сообщение об ошибке
     * @param cause     первопричина ошибки
     */
    public ApplicationException(ApplicationErrorCode errorCode, String message, Throwable cause) {
        super(message, cause);
        this.errorCode = errorCode;
    }

    /**
     * Возвращает код ошибки, связанный с данным исключением.
     *
     * @return код ошибки
     */
    public ApplicationErrorCode getErrorCode() {
        return errorCode;
    }
}
