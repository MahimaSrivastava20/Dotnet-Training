/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: 'class',
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        "on-surface-variant": "#414754",
        "tertiary": "#AB47BC", // Light Pink/Purple
        "primary": "#7E57C2", // Light Purple
        "surface-container": "#F3E5F5", // Very light purple surface
        "on-secondary-container": "#01579B",
        "on-background": "#131b2e",
        "secondary-container": "#E1F5FE", // Very light blue
        "surface-container-high": "#E1BEE7",
        "on-tertiary-fixed": "#4A0072",
        "surface": "#FFFFFF",
        "on-secondary-fixed-variant": "#0277BD",
        "primary-fixed-dim": "#B39DDB",
        "surface-container-lowest": "#ffffff",
        "primary-container": "#512DA8",
        "surface-dim": "#F3E5F5",
        "inverse-on-surface": "#F3E5F5",
        "on-primary": "#ffffff",
        "inverse-primary": "#B39DDB",
        "error-container": "#ffdad6",
        "outline-variant": "#c1c6d6",
        "on-secondary": "#ffffff",
        "secondary": "#29B6F6", // Light Blue
        "on-error-container": "#93000a",
        "secondary-fixed-dim": "#4FC3F7",
        "surface-bright": "#ffffff",
        "on-tertiary-container": "#ffffff",
        "surface-variant": "#E1BEE7",
        "on-tertiary-fixed-variant": "#7B1FA2",
        "error": "#ba1a1a",
        "on-primary-fixed-variant": "#311B92",
        "tertiary-fixed-dim": "#CE93D8",
        "outline": "#727785",
        "surface-container-low": "#F8BBD0",
        "on-surface": "#131b2e",
        "primary-fixed": "#D1C4E9",
        "secondary-fixed": "#B3E5FC",
        "on-primary-container": "#ffffff",
        "tertiary-fixed": "#E1BEE7",
        "on-secondary-fixed": "#01579B",
        "background": "#FDFBFF",
        "surface-tint": "#7E57C2",
        "inverse-surface": "#283044",
        "on-tertiary": "#ffffff",
        "on-primary-fixed": "#311B92",
        "on-error": "#ffffff",
        "surface-container-highest": "#D1C4E9",
        "tertiary-container": "#8E24AA"
      },
      borderRadius: {
        "DEFAULT": "0.25rem",
        "lg": "0.5rem",
        "xl": "0.75rem",
        "full": "9999px"
      },
      spacing: {
        "margin": "32px",
        "lg": "24px",
        "base": "8px",
        "sm": "8px",
        "md": "16px",
        "gutter": "24px",
        "xs": "4px",
        "xl": "32px"
      },
      fontFamily: {
        "display": ["Inter", "sans-serif"],
        "body-sm": ["Inter", "sans-serif"],
        "h3": ["Inter", "sans-serif"],
        "h2": ["Inter", "sans-serif"],
        "body-md": ["Inter", "sans-serif"],
        "label-md": ["Inter", "sans-serif"],
        "h1": ["Inter", "sans-serif"],
        "label-sm": ["Inter", "sans-serif"],
        "body-lg": ["Inter", "sans-serif"]
      },
      fontSize: {
        "display": ["40px", { "lineHeight": "48px", "letterSpacing": "-0.02em", "fontWeight": "700" }],
        "body-sm": ["14px", { "lineHeight": "20px", "fontWeight": "400" }],
        "h3": ["20px", { "lineHeight": "28px", "fontWeight": "600" }],
        "h2": ["24px", { "lineHeight": "32px", "fontWeight": "600" }],
        "body-md": ["16px", { "lineHeight": "24px", "fontWeight": "400" }],
        "label-md": ["12px", { "lineHeight": "16px", "letterSpacing": "0.05em", "fontWeight": "600" }],
        "h1": ["32px", { "lineHeight": "40px", "letterSpacing": "-0.01em", "fontWeight": "600" }],
        "label-sm": ["11px", { "lineHeight": "14px", "fontWeight": "500" }],
        "body-lg": ["18px", { "lineHeight": "28px", "fontWeight": "400" }]
      }
    },
  },
  plugins: [
    require('@tailwindcss/forms'),
    require('@tailwindcss/container-queries')
  ],
}
