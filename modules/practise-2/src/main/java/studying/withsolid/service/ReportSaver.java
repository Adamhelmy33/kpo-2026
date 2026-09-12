package studying.withsolid.service;

import studying.withsolid.exception.ApplicationException;
import studying.withsolid.model.Report;

/**
 * Отвечает за сохранение отчета.
 */
public interface ReportSaver {

    /**
     * Сохраняет отчет.
     *
     * @param report отчет для сохранения
     * @throws ApplicationException если отчет не удалось сохранить
     */
    void save(Report report);
}
