import { Camera } from 'react-native-camera';

interface BarcodeScannerProps {
  onScan: (barcode: string) => void;
  onClose: () => void;
}

export const BarcodeScanner: React.FC<BarcodeScannerProps> = ({
  onScan,
  onClose
}) => {
  // Implementación del escáner con react-native-camera
  // - Detectar códigos de barras 1D/2D
  // - Feedback visual/sonoro
  // - Modo linterna
  // - Captura manual si falla escaneo
};