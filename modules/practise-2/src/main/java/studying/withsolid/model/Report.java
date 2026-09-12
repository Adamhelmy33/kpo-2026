package studying.withsolid.model;

import studying.withsolid.exception.ApplicationErrorCode;
import studying.withsolid.exception.ApplicationException;

import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

/**
 * Неизменяемая модель отчета о продажах.
 *
 * @param dateTime         дата и время формирования отчета
 * @param carsSold         количество проданных автомобилей
 * @param motorcyclesSold  количество проданных мотоциклов
 */
public record Report(LocalDateTime dateTime, int carsSold, int motorcyclesSold) {

    private static final DateTimeFormatter DISPLAY_FORMATTER = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");

    /**
     * Проверяет корректность данных отчета.
     *
     * @throws ApplicationException с кодом {@link ApplicationErrorCode#VALIDATION_ERROR},
     *                              если дата и время не заданы или показатели продаж отрицательны
     */
    public Report {
        if (dateTime == null) {
            throw new ApplicationException(ApplicationErrorCode.VALIDATION_ERROR,
                    "Дата и время отчета не должны быть null");
        }
        if (carsSold < 0) {
            throw new ApplicationException(ApplicationErrorCode.VALIDATION_ERROR,
                    "Количество проданных автомобилей не может быть отрицательным");
        }
        if (motorcyclesSold < 0) {
            throw new ApplicationException(ApplicationErrorCode.VALIDATION_ERROR,
                    "Количество проданных мотоциклов не может быть отрицательным");
        }
    }

    /**
     * Формирует текстовое представление отчета.
     *
     * @return текст отчета
     */
    public String toReportText() {
        return """
                Отчет о продажах
                Дата и время: %s
                Продано автомобилей: %d
                Продано мотоциклов: %d
                """.formatted(dateTime.format(DISPLAY_FORMATTER), carsSold, motorcyclesSold);
    }
}
