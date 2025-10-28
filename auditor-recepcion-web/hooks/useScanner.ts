import { useState, useCallback, useRef, useEffect } from 'react';
import { Html5Qrcode } from 'html5-qrcode';
import { productosService } from '@services/productos.service';
import { ProductoInfo } from '@types/auditoria.types';

interface UseScannerOptions {
  onScanSuccess?: (producto: ProductoInfo) => void;
  onScanError?: (error: string) => void;
  autoStart?: boolean;
}

export const useScanner = (options: UseScannerOptions = {}) => {
  const [isScanning, setIsScanning] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [lastScannedCode, setLastScannedCode] = useState<string>('');
  const [producto, setProducto] = useState<ProductoInfo | null>(null);

  const scannerRef = useRef<Html5Qrcode | null>(null);
  const elementIdRef = useRef<string>('barcode-scanner-container');

  /**
   * Iniciar escáner
   */
  const startScanner = useCallback(async (elementId?: string) => {
    if (elementId) {
      elementIdRef.current = elementId;
    }

    try {
      setIsLoading(true);
      setError(null);

      // Crear instancia del escáner
      if (!scannerRef.current) {
        scannerRef.current = new Html5Qrcode(elementIdRef.current);
      }

      // Configuración del escáner
      const config = {
        fps: 10,
        qrbox: { width: 250, height: 250 },
        aspectRatio: 1.0,
      };

      // Callback cuando se escanea exitosamente
      const onScanSuccess = async (decodedText: string) => {
        if (decodedText === lastScannedCode) return; // Evitar duplicados

        setLastScannedCode(decodedText);
        setIsLoading(true);

        try {
          // Buscar producto por código de barras
          const productoEncontrado = await productosService.getProductoByBarcode(decodedText);
          setProducto(productoEncontrado);

          if (options.onScanSuccess) {
            options.onScanSuccess(productoEncontrado);
          }
        } catch (error: any) {
          const errorMsg = 'Producto no encontrado';
          setError(errorMsg);
          
          if (options.onScanError) {
            options.onScanError(errorMsg);
          }
        } finally {
          setIsLoading(false);
        }
      };

      // Callback de error del escáner
      const onScanError = (errorMessage: string) => {
        // Los errores continuos del escáner son normales, solo loguear
        console.debug('Scanner error:', errorMessage);
      };

      // Iniciar el escáner
      await scannerRef.current.start(
        { facingMode: 'environment' }, // Usar cámara trasera
        config,
        onScanSuccess,
        onScanError
      );

      setIsScanning(true);
      setIsLoading(false);
    } catch (err: any) {
      const errorMsg = err.message || 'Error al iniciar el escáner';
      setError(errorMsg);
      setIsLoading(false);
      
      if (options.onScanError) {
        options.onScanError(errorMsg);
      }
    }
  }, [lastScannedCode, options]);

  /**
   * Detener escáner
   */
  const stopScanner = useCallback(async () => {
    if (scannerRef.current && isScanning) {
      try {
        await scannerRef.current.stop();
        setIsScanning(false);
      } catch (err) {
        console.error('Error stopping scanner:', err);
      }
    }
  }, [isScanning]);

  /**
   * Cambiar cámara (frontal/trasera)
   */
  const switchCamera = useCallback(async () => {
    if (!scannerRef.current) return;

    try {
      await stopScanner();
      // Implementar lógica para cambiar cámara
      // Esto requeriría reiniciar el escáner con diferente configuración
      await startScanner();
    } catch (err) {
      console.error('Error switching camera:', err);
    }
  }, [stopScanner, startScanner]);

  /**
   * Buscar producto manualmente (sin escáner)
   */
  const searchByCode = useCallback(async (code: string) => {
    setIsLoading(true);
    setError(null);

    try {
      const productoEncontrado = await productosService.getProductoByBarcode(code);
      setProducto(productoEncontrado);
      setLastScannedCode(code);

      if (options.onScanSuccess) {
        options.onScanSuccess(productoEncontrado);
      }

      return productoEncontrado;
    } catch (error: any) {
      const errorMsg = 'Producto no encontrado';
      setError(errorMsg);
      
      if (options.onScanError) {
        options.onScanError(errorMsg);
      }
      
      throw error;
    } finally {
      setIsLoading(false);
    }
  }, [options]);

  /**
   * Limpiar estado
   */
  const reset = useCallback(() => {
    setProducto(null);
    setLastScannedCode('');
    setError(null);
  }, []);

  // Cleanup al desmontar
  useEffect(() => {
    return () => {
      if (scannerRef.current && isScanning) {
        scannerRef.current.stop().catch(console.error);
      }
    };
  }, [isScanning]);

  // Auto-start si está configurado
  useEffect(() => {
    if (options.autoStart) {
      startScanner();
    }
  }, [options.autoStart]);

  return {
    // Estado
    isScanning,
    isLoading,
    error,
    lastScannedCode,
    producto,

    // Acciones
    startScanner,
    stopScanner,
    switchCamera,
    searchByCode,
    reset,
  };
};