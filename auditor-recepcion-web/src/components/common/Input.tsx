import React from 'react';
import { TextField, TextFieldProps } from '@mui/material';

interface InputProps extends Omit<TextFieldProps, 'variant'> {
  variant?: 'outlined' | 'filled' | 'standard';
  errorText?: string;
}

export const Input: React.FC<InputProps> = ({
  variant = 'outlined',
  error,
  errorText,
  helperText,
  fullWidth = true,
  ...props
}) => {
  return (
    <TextField
      variant={variant}
      error={error}
      helperText={errorText || helperText}
      fullWidth={fullWidth}
      {...props}
    />
  );
};