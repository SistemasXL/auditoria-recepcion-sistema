import React from 'react';
import { Alert as MuiAlert, AlertTitle, AlertProps as MuiAlertProps } from '@mui/material';

interface AlertProps extends MuiAlertProps {
  title?: string;
  message?: string;
}

export const Alert: React.FC<AlertProps> = ({ 
  title, 
  message,
  children,
  severity = 'info',
  ...props 
}) => {
  return (
    <MuiAlert severity={severity} {...props}>
      {title && <AlertTitle>{title}</AlertTitle>}
      {message || children}
    </MuiAlert>
  );
};