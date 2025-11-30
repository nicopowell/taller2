using System.Collections.Generic;
using MVC.Models; 

namespace MVC.Interfaces
{
    // ====================================================================================
    // INTERFAZ DEL REPOSITORIO DE PRESUPUESTOS
    // ====================================================================================
    // CONCEPTO TEÓRICO: Patrón Repositorio (TP 9/10) & Relaciones
    //
    // 1. Objetivo:
    //    Definir las operaciones CRUD para la entidad 'Presupuesto'.
    //    Esta interfaz será inyectada en 'PresupuestosController'.
    //
    // 2. Relación con Productos:
    //    Además del CRUD básico, incluye 'AddDetalle' para gestionar la tabla intermedia
    //    'PresupuestoDetalle' (Relación Muchos a Muchos).
    // ====================================================================================

    public interface IPresupuestoRepository
    {
        // --- LECTURA ---
        
        // Devuelve todos los presupuestos (cabeceras).
        // Usado en la acción Index.
        List<Presupuesto> GetAll();
        
        // Devuelve un presupuesto completo por ID.
        // IMPORTANTE: Este método DEBE incluir la carga de los 'Detalles' (Productos asociados).
        // Usado en la acción Details.
        Presupuesto GetById(int id);
        
        // --- ESCRITURA (CABECERA) ---
        
        // Crea un nuevo presupuesto (solo datos generales: Nombre Destinatario, Fecha).
        // Usado en Create (Post).
        void Add(Presupuesto presupuesto);
        
        // Actualiza los datos de cabecera del presupuesto.
        // Usado en Edit (Post).
        void Update(Presupuesto presupuesto);
        
        // Elimina el presupuesto Y sus detalles asociados (Integridad Referencial).
        // Usado en Delete (Post).
        void Delete(int id);
        
        // --- ESCRITURA (DETALLES / RELACIÓN) ---
        
        // Método clave del TP para la relación N:M.
        // Agrega un producto específico con una cantidad a un presupuesto existente.
        // Inserta en la tabla intermedia 'PresupuestoDetalle'.
        // Usado en la acción 'AgregarProducto' del controlador.
        void AddDetalle(int idPresupuesto, int idProducto, int cantidad);
    }
}