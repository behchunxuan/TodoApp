import React from "react";
import TodoList from "./components/TodoList";

function App() {
  return (
    <div className="max-w-5xl mx-auto py-8">
      <h1 className="text-3xl font-bold mb-6 text-center">📝 To Do Dashboard</h1>
      <TodoList />
    </div>
  );
}

export default App;