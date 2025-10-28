import React from 'react';
import {
  FormControl,
  InputLabel,
  Select as MuiSelect,
  MenuItem,
  FormHelperText,
  SelectProps as MuiSelectProps,
} from '@mui/material';

export interface SelectOption {
  value: string | number;
  label: string;
  disabled?: boolean;
}

interface SelectProps extends Omit<MuiSelectProps, 'variant'> {
  label?: string;
  options: SelectOption[];
  errorText?: string;
  helperText?: string;
  variant?: 'outlined' | 'filled' | 'standard';
}

export const Select: React.FC<SelectProps> = ({
  label,
  options,
  error,
  errorText,
  helperText,
  fullWidth = true,
  variant = 'outlined',
  ...props
}) => {
  return (
    <FormControl fullWidth={fullWidth} error={error} variant={variant}>
      {label && <InputLabel>{label}</InputLabel>}
      <MuiSelect label={label} {...props}>
        {options.map((option) => (
          <MenuItem 
            key={option.value} 
            value={option.value}
            disabled={option.disabled}
          >
            {option.label}
          </MenuItem>
        ))}
      </MuiSelect>
      {(errorText || helperText) && (
        <FormHelperText>{errorText || helperText}</FormHelperText>
      )}
    </FormControl>
  );
};