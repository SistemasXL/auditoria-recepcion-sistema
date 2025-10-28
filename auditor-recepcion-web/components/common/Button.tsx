import React from 'react';
import { Button as MuiButton, ButtonProps as MuiButtonProps, CircularProgress } from '@mui/material';

interface ButtonProps extends Omit<MuiButtonProps, 'variant'> {
  variant?: 'contained' | 'outlined' | 'text';
  loading?: boolean;
  fullWidth?: boolean;
  icon?: React.ReactNode;
}

export const Button: React.FC<ButtonProps> = ({
  children,
  loading = false,
  disabled,
  fullWidth = false,
  variant = 'contained',
  icon,
  startIcon,
  ...props
}) => {
  return (
    <MuiButton
      variant={variant}
      disabled={disabled || loading}
      fullWidth={fullWidth}
      startIcon={loading ? <CircularProgress size={20} color="inherit" /> : (icon || startIcon)}
      {...props}
    >
      {children}
    </MuiButton>
  );
};